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
namespace AwaraIT.BCS.Plugins2.Emails
{
    public class IncomingEmailAttachmentSave : PluginBase
    {
        public IncomingEmailAttachmentSave()
        {
            Subscribe
                //.ToMessage(CrmMessage.Create)
                //.ForEntity("email")
                //.When(PluginStage.PostOperation)
                //.Execute(Execute);
                .ToMessage(CrmMessage.Update)
                .ForEntity("email")
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            var ctx = wrapper.Context;
            var service = wrapper.GetOrganizationService(null);
            var logger = new Logger(service);

            if (ctx.Depth > 1)
                return;

            string EmailDownloadAttachment = CrmEnvironmentVariables.GetSharepointPathUrl(service, "awr_SharePointdownloadfile");
            string CaseUpdateSharePoint = CrmEnvironmentVariables.GetSharepointPathUrl(service, "awr_CaseSharePointfilesUpdate");

            try
            {
                var email = service.Retrieve("email", ctx.PrimaryEntityId,
                    new ColumnSet("subject", "directioncode", "regardingobjectid"));

                var regarding = email.GetAttributeValue<EntityReference>("regardingobjectid");
                if (regarding == null || regarding.LogicalName != "incident") return;
                bool isOutgoing = email.GetAttributeValue<bool>("directioncode");
                if (isOutgoing) return;

                string folderPath = ResolveFolderPath(email, service, logger, regarding);

                var query = new QueryExpression("activitymimeattachment")
                {
                    ColumnSet = new ColumnSet("activitymimeattachmentid", "filename", "body")
                };
                query.Criteria.AddCondition("activityid", ConditionOperator.Equal, email.Id);

                var attachments = service.RetrieveMultiple(query);

                if (attachments.Entities.Count == 0) return;

                var savedFiles = new List<string>();

                foreach (var att in attachments.Entities)
                {
                    try
                    {
                        string filename = att.GetAttributeValue<string>("filename");
                        string base64 = att.GetAttributeValue<string>("body")
                                          ?.Replace("\r", "")
                                          ?.Replace("\n", "");

                        if (string.IsNullOrWhiteSpace(base64)) continue;

                        UploadToSharePoint(folderPath, filename, base64, EmailDownloadAttachment);

                        savedFiles.Add($"{folderPath}/{filename}");

                        service.Delete("activitymimeattachment", att.Id);
                    }
                    catch (Exception exAtt)
                    {
                        logger.ERROR(exAtt, $"Failed Processing Attachment ID: {att.Id}", "email", email.Id);
                    }
                }
                if (savedFiles.Count == 0) return;
                if (savedFiles.Count > 0)
                {
                    var updateEmail = new Entity("email", email.Id);
                    updateEmail["awr_sharepointfiles"] =
                        System.Text.Json.JsonSerializer.Serialize(savedFiles);

                    service.Update(updateEmail);
                }

                CallCaseUpdateFlow(regarding.Id, CaseUpdateSharePoint);
            }
            catch (Exception ex)
            {
                logger.CRITICAL("IncomingEmailAttachmentSave error", ex.ToString(), "email", wrapper.Context.PrimaryEntityId);
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private string ResolveFolderPath(Entity email, IOrganizationService service, Logger logger, EntityReference regarding)
        {

                Guid caseId = regarding.Id;

                var caseEntity = service.Retrieve("incident", caseId, new ColumnSet("title"));
                string title = caseEntity.GetAttributeValue<string>("title") ?? "Case";

                string folder = $"/incident/{title}_{caseId.ToString().Replace("-", "").ToUpper()}";

                return folder;
            
        }

        private void UploadToSharePoint(string folder, string fileName, string base64 , string flowUrl)
        {
            var payload = new
            {
                folderPath = folder,
                fileName = fileName,
                fileContent = base64
            };

            using (var client = new HttpClient())
            {
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var resp = client.PostAsync(flowUrl, content).Result;

                if (!resp.IsSuccessStatusCode)
                    throw new Exception($"SharePoint upload failed: {resp.StatusCode}");
            }
        }

        private void CallCaseUpdateFlow(Guid caseId, string flowUrl)
        {
            var payload = new
            {
                case_id = caseId.ToString()
            };

            using (var client = new HttpClient())
            {
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = client.PostAsync(flowUrl, content).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidPluginExecutionException(
                        $"Case SharePoint update Flow failed. Status: {response.StatusCode}"
                    );
                }
            }
        }

    }
}
