using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Domain.Models.Crm;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class RoutingRuleElementRepository : CrmRepository<RoutingRuleElement>
    {
        public RoutingRuleElementRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => RoutingRuleElement.EntityLogicalName;

        public List<RoutingRuleElement> GetByEntityName(string name, bool byMainLink = true) 
        {
            var query = new QueryExpression(RoutingRuleElement.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(RoutingRuleElement.Metadata.RoutingRulesSetId,
                                          RoutingRuleElement.Metadata.SystemUserId,
                                          RoutingRuleElement.Metadata.FetchFilter,
                                          RoutingRuleElement.Metadata.EntityName,
                                          RoutingRuleElement.Metadata.AssignAt,
                                          RoutingRuleElement.Metadata.Priority,
                                          RoutingRuleElement.Metadata.TeamId,
                                          RoutingRuleElement.Metadata.Name,
                                          RoutingRuleElement.Metadata.Path)
            };
            query.Criteria.AddCondition(EntityCommon.StateCode, ConditionOperator.Equal, 0);

            if (byMainLink)
            {
                var routingRuleSetLink = query.AddLink(RoutingRulesSet.EntityLogicalName, 
                                                       RoutingRuleElement.Metadata.RoutingRulesSetId, 
                                                       RoutingRulesSet.Metadata.RoutingRulesSetId);
                routingRuleSetLink.EntityAlias = "routingRuleSetLink";
                routingRuleSetLink.Columns.AddColumn(RoutingRulesSet.Metadata.Name);
                routingRuleSetLink.LinkCriteria.AddCondition(RoutingRulesSet.Metadata.EntityName, ConditionOperator.Equal, name);
                routingRuleSetLink.LinkCriteria.AddCondition(EntityCommon.StateCode, ConditionOperator.Equal, 0);
            }
            else
                query.Criteria.AddCondition(RoutingRuleElement.Metadata.EntityName, ConditionOperator.Equal, name);

            var result = Get(query).OrderBy(e => e.Priority).ToList();

            return result;
        }
    }
}