using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain.Models.Crm.EnvironmentVariables;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace AwaraIT.BCS.Plugins2.Emails
{
    public class EmailAttachmentAdd : PluginBase
    {
         public EmailAttachmentAdd()
        {
            Subscribe
                //.ToMessage("Send")
                //.ForEntity("email")
                //.When(PluginStage.PreOperation)
                //.Execute(Execute);

                .ToMessage(CrmMessage.Update)
                .ForEntity("email")
                .When(PluginStage.PreOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            var ctx = wrapper.Context;
            var service = wrapper.GetOrganizationService(null);
            var logger = new Logger(service);

            string flowUrl = CrmEnvironmentVariables.GetSharepointPathUrl(service, "awr_SharePointGetFileContent");


            try
            {
                var target = (Entity)ctx.InputParameters["Target"];

                EntityReference regarding = null;

                if (target.Contains("regardingobjectid"))
                {
                    regarding = target.GetAttributeValue<EntityReference>("regardingobjectid");
                }
                else if (ctx.PrimaryEntityId != Guid.Empty)
                {
                    var email = service.Retrieve(
                        "email",
                        ctx.PrimaryEntityId,
                        new ColumnSet("regardingobjectid")
                    );
                    regarding = email.GetAttributeValue<EntityReference>("regardingobjectid");
                }
                if (regarding == null || regarding.LogicalName != "incident") return;


                if (!target.Contains("awr_sharepointfiles")) return;

                string spFilesJson = target.GetAttributeValue<string>("awr_sharepointfiles");

                List<string> fileIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(spFilesJson);
                if (fileIds == null || fileIds.Count == 0) return;

                using (var http = new HttpClient())
                {
                    foreach (var fileId in fileIds)
                    {
                        try
                        {
                            var payload = new { fileId };
                            var json = System.Text.Json.JsonSerializer.Serialize(payload);
                            var content = new StringContent(json, Encoding.UTF8, "application/json");

                            var resp = http.PostAsync(flowUrl, content).Result;

                            if (!resp.IsSuccessStatusCode)
                            {
                                logger.ERROR($"Flow returned status code {resp.StatusCode} for fileId {fileId}",
                                             entityType: target.LogicalName, entityid: target.Id);
                                continue;
                            }

                            var bytes = resp.Content.ReadAsByteArrayAsync().Result;
                            string base64 = Convert.ToBase64String(bytes);
                            string mime = resp.Content.Headers.ContentType?.MediaType
                                           ?? "application/octet-stream";

                            string filename = ExtractSafeFilename(fileId);

                            // --- Check if attachment already exists ---
                            var existingQuery = new QueryExpression("activitymimeattachment")
                            {
                                ColumnSet = new ColumnSet("activitymimeattachmentid")
                            };
                            existingQuery.Criteria.AddCondition("activityid", ConditionOperator.Equal, ctx.PrimaryEntityId);
                            existingQuery.Criteria.AddCondition("filename", ConditionOperator.Equal, filename);

                            var existing = service.RetrieveMultiple(existingQuery);

                            if (existing.Entities.Count > 0)
                            {
                                continue;
                            }

                            // ---  Create new attachment ---
                            var attach = new Entity("activitymimeattachment");
                            attach["activityid"] = new EntityReference("email", ctx.PrimaryEntityId);
                            attach["filename"] = filename;
                            attach["body"] = base64;
                            attach["mimetype"] = mime;

                            service.Create(attach);

                        }
                        catch (Exception exFile)
                        {
                            logger.ERROR(exFile, $"Error processing fileId {fileId}", entityType: target.LogicalName, entityid: target.Id);

                            throw new InvalidPluginExecutionException($"EmailAttachmentAdd failed: {exFile.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.CRITICAL(ex.Message, ex.ToString());
                throw new InvalidPluginExecutionException($"EmailAttachmentAdd failed: {ex.Message}");
            }
        }


        private static string ExtractSafeFilename(string fileId)
        {
            string decoded = Uri.UnescapeDataString(fileId);
            decoded = Uri.UnescapeDataString(decoded);

            string filename = decoded.Substring(decoded.LastIndexOf('/') + 1);

            if (filename.Length > 255)
            {
                string ext = "";
                int dot = filename.LastIndexOf(".");
                if (dot > 0) ext = filename.Substring(dot);

                string baseName = filename.Substring(0, 255 - ext.Length);
                filename = baseName + ext;
            }

            return filename;
        }
    }
}