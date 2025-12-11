using AwaraIT.BCS.Application.Features.Leads;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;

namespace AwaraIT.BCS.Plugins.Leads
{
    public class DuplicateCheckerRfCRMPlugin : PluginBase
    {
        public DuplicateCheckerRfCRMPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(Lead.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
            Subscribe
                .ToMessage(CrmMessage.Update)
                .ForEntity(Lead.EntityLogicalName)
                .WithAnyField(Lead.Metadata.MobilePhone, Lead.Metadata.Email)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            var service = wrapper.GetOrganizationService(null);
            new DuplicateCheckerRfCRMService(service).Execute(wrapper.TargetEntity.Id);
        }
    }
}
