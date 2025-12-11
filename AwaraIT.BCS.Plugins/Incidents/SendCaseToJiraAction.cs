using AwaraIT.BCS.Application.Features.Incidents;
using AwaraIT.BCS.Plugins.PluginExtensions;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace AwaraIT.BCS.Plugins.Incidents
{
    public class SendCaseToJiraAction : BasicActivity
    {
        [RequiredArgument]
        [Input("ProjectName")]
        public InArgument<string> ProjectName { get; set; }

        [Output("ErrorMessage")]
        public OutArgument<string> ErrorMessage { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            var projectName = ProjectName.Get(executionContext);

            base.Execute(executionContext);
            ErrorMessage.Set(executionContext, string.Empty);

            var res = new SendCaseToJiraService(CrmService).Execute(WorkflowContext.PrimaryEntityId, projectName);
            if (!res.IsSuccess) ErrorMessage.Set(executionContext, res.Error);
        }
    }
}
