using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace AwaraIT.BCS.Plugins.PluginExtensions
{
    public abstract class BasicActivity : CodeActivity
    {
        public IOrganizationService CrmService { get; set; }
        internal IWorkflowContext WorkflowContext { get; set; }

        protected override void Execute(CodeActivityContext executionContext)
        {
            var context = executionContext.GetExtension<IWorkflowContext>();
            var serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(context.UserId);

            CrmService = service;
            WorkflowContext = context;
        }
    }
}
