using AwaraIT.BCS.Application.Features.PushMessages;
using AwaraIT.BCS.Domain.Models.Crm;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;

namespace AwaraIT.BCS.Plugins.PushMessages
{
    public class UpdateMarketingMessageStatusPlugin : PluginBase
    {
        public UpdateMarketingMessageStatusPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Update)
                .ForEntity(PushMessage.EntityLogicalName)
                .WithAnyField(EntityCommon.StatusCode)
                .PreImageRequired(PushMessage.Metadata.MarketingId, PushMessage.Metadata.ChannelDefinitionId, PushMessage.Metadata.MarketingSender)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            new UpdateMarketingMessageStatusService(wrapper.Service).Execute(
                wrapper.TargetEntity.ToEntity<PushMessage>(),
                wrapper.PreImage.ToEntity<PushMessage>(),
                wrapper.Context.OrganizationId);
        }
    }
}
