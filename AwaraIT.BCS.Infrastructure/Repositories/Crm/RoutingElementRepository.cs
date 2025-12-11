using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class RoutingElementRepository : CrmRepository<RoutingElement>
    {
        public RoutingElementRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => RoutingElement.EntityLogicalName;

        public Guid CreateByRoutingRuleElement(EntityReference targetRef, RoutingRuleElement routingRuleElement, EntityReference newOwner, bool isActivity = false) 
        {
            var element = new RoutingElement();
            element.Subject = $"Success routing {targetRef.LogicalName}, id: {targetRef.Id}, isActivity: {isActivity}";
            element.RoutingRulesSetId = routingRuleElement.RoutingRulesSetId;
            element.RoutingRuleElementId = routingRuleElement.ToEntityReference();

            if (!isActivity)
                element.RegardingObjectId = targetRef;

            if (newOwner.LogicalName == "systemuser")
                element.SystemUserId = newOwner;
            else
                element.TeamId = newOwner;

            return Create(element);
        }
    }
}