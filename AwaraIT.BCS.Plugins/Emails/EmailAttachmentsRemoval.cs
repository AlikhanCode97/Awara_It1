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
    public class EmailAttachmentsRemoval : PluginBase
    {
        public EmailAttachmentsRemoval()
        {
            Subscribe
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

            try
            {
                var target = ctx.InputParameters.Contains("Target") ? ctx.InputParameters["Target"] as Entity : null;

                EntityReference regarding = null;

                if (!target.Contains("statuscode") && !target.Contains("statecode"))
                    return;


                var email = service.Retrieve(
                    "email",
                    ctx.PrimaryEntityId,
                    new ColumnSet("directioncode", "regardingobjectid", "statuscode")
                );

                int status = email.GetAttributeValue<OptionSetValue>("statuscode")?.Value ?? -1;
                if (status != 3 && status != 5) return;

                regarding = email.GetAttributeValue<EntityReference>("regardingobjectid");
                if (regarding == null || regarding.LogicalName != "incident") return;

                bool isOutgoing = email.GetAttributeValue<bool>("directioncode");
                if (!isOutgoing)return;

                var query = new QueryExpression("activitymimeattachment")
                {
                    ColumnSet = new ColumnSet("activitymimeattachmentid", "filename")
                };
                query.Criteria.AddCondition("activityid", ConditionOperator.Equal, target.Id);

                var attachments = service.RetrieveMultiple(query);

                foreach (var att in attachments.Entities)
                {
                    string name = att.GetAttributeValue<string>("filename");

                    service.Delete("activitymimeattachment", att.Id);
                }

            }
            catch (Exception ex)
            {
                logger.ERROR(ex, "Error in EmailAttachmentCleanup", "email", wrapper.Context.PrimaryEntityId);
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }
    }
}