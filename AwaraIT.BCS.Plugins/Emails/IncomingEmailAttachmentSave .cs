using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Net.Http;
using System.Text;

namespace AwaraIT.BCS.Plugins2.Emails
{
    public class IncomingEmailAttachmentSave : PluginBase
    {
        private readonly string flowUrl =
            "https://4bdff9edf2e9ef33ba314ad199dd94.17.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/adadb9de9c5749618b93c93f85669d81/triggers/manual/paths/invoke?api-version=1&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=fLxddIHA4xxGwRg1nhi4oqBmjH0QNS9VWLhY-h-R4hg";

        public IncomingEmailAttachmentSave()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity("email")
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            var ctx = wrapper.Context;
            var service = wrapper.GetOrganizationService(null);
            var logger = new Logger(service);

            try
            {
                var email = service.Retrieve("email", ctx.PrimaryEntityId,
                    new ColumnSet("subject", "directioncode", "regardingobjectid"));

                bool incoming = email.GetAttributeValue<bool>("directioncode");
                if (!incoming) return;

                string folderPath = ResolveFolderPath(email, service, logger);

                var query = new QueryExpression("activitymimeattachment")
                {
                    ColumnSet = new ColumnSet("activitymimeattachmentid", "filename", "body")
                };
                query.Criteria.AddCondition("activityid", ConditionOperator.Equal, email.Id);

                var attachments = service.RetrieveMultiple(query);

                if (attachments.Entities.Count == 0) return;

                foreach (var att in attachments.Entities)
                {
                    try
                    {
                        string filename = att.GetAttributeValue<string>("filename");
                        string base64 = att.GetAttributeValue<string>("body")
                                          ?.Replace("\r", "")
                                          ?.Replace("\n", "");

                        UploadToSharePoint(folderPath, filename, base64);

                        service.Delete("activitymimeattachment", att.Id);
                    }
                    catch (Exception exAtt)
                    {
                        logger.ERROR(exAtt, $"Failed Processing Attachment ID: {att.Id}", "email", email.Id);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.CRITICAL("IncomingEmailAttachmentSave error", ex.ToString(), "email", wrapper.Context.PrimaryEntityId);
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private string ResolveFolderPath(Entity email, IOrganizationService service, Logger logger)
        {
            var regarding = email.GetAttributeValue<EntityReference>("regardingobjectid");

            if (regarding != null && regarding.LogicalName == "incident")
            {
                Guid caseId = regarding.Id;

                var caseEntity = service.Retrieve("incident", caseId, new ColumnSet("title"));
                string title = caseEntity.GetAttributeValue<string>("title") ?? "Case";

                string folder = $"/incident/{title}_{caseId.ToString().Replace("-", "").ToUpper()}";

                return folder;
            }

            string subject = email.GetAttributeValue<string>("subject") ?? "NoSubject";
            return $"/email/{subject}_{email.Id.ToString().Replace("-", "").ToUpper()}";
        }

        private void UploadToSharePoint(string folder, string fileName, string base64)
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
    }
}
