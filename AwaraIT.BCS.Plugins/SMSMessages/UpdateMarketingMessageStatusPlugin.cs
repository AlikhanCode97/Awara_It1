using AwaraIT.BCS.Application.Features.SMSMessages;
using AwaraIT.BCS.Domain.Models.Crm;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;

namespace AwaraIT.BCS.Plugins.SMSMessages
{
    public class UpdateMarketingMessageStatusPlugin : PluginBase
    {
        public UpdateMarketingMessageStatusPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Update)
                .ForEntity(SMSMessage.EntityLogicalName)
                .WithAnyField(EntityCommon.StatusCode)
                .PreImageRequired(SMSMessage.Metadata.MarketingId, SMSMessage.Metadata.ChannelDefinitionId, SMSMessage.Metadata.MarketingSender)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            new UpdateMarketingMessageStatusService(wrapper.Service).Execute(
                wrapper.TargetEntity.ToEntity<SMSMessage>(),
                wrapper.PreImage.ToEntity<SMSMessage>(),
                wrapper.Context.OrganizationId);
        }
    }
}
