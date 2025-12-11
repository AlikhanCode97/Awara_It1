using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Application.Features.Leads;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;


namespace AwaraIT.BCS.Plugins.Leads
{
    public class BindContactToLeadPlugin : PluginBase
    {
        private Logger _logger;
        public BindContactToLeadPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(Lead.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            _logger = new Logger(wrapper.Service);
            var lead = wrapper.TargetEntity.ToEntity<Lead>();

            var res = new BindContactToLeadService(wrapper.Service).Execute(lead);
            if (!res.IsSuccess)
            {
                _logger.ERROR(res.Error);
            }
        }
    }
}
