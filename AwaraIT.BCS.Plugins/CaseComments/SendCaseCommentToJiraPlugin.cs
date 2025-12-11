using AwaraIT.BCS.Application.Features.CaseComments;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Plugins.PluginExtensions;
using AwaraIT.BCS.Plugins.PluginExtensions.Enums;
using AwaraIT.BCS.Plugins.PluginExtensions.Interfaces;

namespace AwaraIT.BCS.Plugins.CaseComments
{
    public class SendCaseCommentToJiraPlugin : PluginBase
    {
        public SendCaseCommentToJiraPlugin()
        {
            Subscribe
                .ToMessage(CrmMessage.Create)
                .ForEntity(CaseComment.EntityLogicalName)
                .When(PluginStage.PostOperation)
                .Execute(Execute);
        }

        private void Execute(IContextWrapper wrapper)
        {
            var caseComment = wrapper.TargetEntity.ToEntity<CaseComment>();
            new SendCaseCommentToJiraService(wrapper.Service).Execute(caseComment);
        }
    }
}
