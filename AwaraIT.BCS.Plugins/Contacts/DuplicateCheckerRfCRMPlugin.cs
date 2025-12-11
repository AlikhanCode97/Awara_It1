using AwaraIT.BCS.Application.Features.Contacts;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;

namespace AwaraIT.BCS.Plugins.Contacts
{
    public class DuplicateCheckerRfCRMPlugin : PluginBase
    {
        public DuplicateCheckerRfCRMPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(Contact.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
            Subscribe
                .ToMessage(CrmMessage.Update)
                .ForEntity(Contact.EntityLogicalName)
                .WithAnyField(Contact.Metadata.MobilePhone, Contact.Metadata.Email, Contact.Metadata.Email2, Contact.Metadata.Email3,
                    Contact.Metadata.Telephone1, Contact.Metadata.Telephone2, Contact.Metadata.Telephone3)
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
