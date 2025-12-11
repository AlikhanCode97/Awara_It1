using AwaraIT.BCS.Application.Core;
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
        private readonly string flowUrl = "https://4bdff9edf2e9ef33ba314ad199dd94.17.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/309f51f6ca33492dae6724499e388596/triggers/manual/paths/invoke?api-version=1&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=RCdsWDr5htYmoZ0jglL82eU526Nunwd-S5RUC00Dznw";
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

            
            try
            {
                var target = (Entity)ctx.InputParameters["Target"];
                var emailRef = service.Retrieve("email", target.Id, new ColumnSet("awr_sharepointfiles"));

                string spFilesJson = emailRef.GetAttributeValue<string>("awr_sharepointfiles");

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

                            var attach = new Entity("activitymimeattachment");
                            attach["activityid"] = emailRef.ToEntityReference();
                            attach["filename"] = filename;
                            attach["body"] = base64;
                            attach["mimetype"] = mime;

                            service.Create(attach);
                        }
                        catch (Exception exFile)
                        {
                            logger.ERROR(exFile, $"Error processing fileId {fileId}", entityType: target.LogicalName, entityid: target.Id);
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
            decoded = Uri.UnescapeDataString(decoded); // double decode

            string filename = decoded.Substring(decoded.LastIndexOf('/') + 1);

            // Enforce CRM limit 255 chars
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