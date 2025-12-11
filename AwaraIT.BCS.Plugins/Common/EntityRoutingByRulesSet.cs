using AwaraIT.BCS.Application.Features.Common;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
using System.Activities;
using System;

namespace AwaraIT.BCS.Plugins.Common
{
    public class EntityRoutingByRulesSet : CodeActivity
    {
        [Output("Success operation")]
        [Default("False")]
        public OutArgument<bool> isSuccess { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var workflowContext = context.GetExtension<IWorkflowContext>();
            var serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            var service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            var currentEntityRef = new EntityReference(workflowContext.PrimaryEntityName, workflowContext.PrimaryEntityId);
            if (currentEntityRef == null)
            {
                isSuccess.Set(context, false);
                return;
            }

            try 
            {
                var result = new RuleSetRoutingService(service, currentEntityRef).Run();
                isSuccess.Set(context, result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while processing entity {currentEntityRef.LogicalName}, id: {currentEntityRef.Id}. Error message: {ex.Message}. Inner error message: {ex.InnerException?.Message}.", ex);
            }
        }
    }
}