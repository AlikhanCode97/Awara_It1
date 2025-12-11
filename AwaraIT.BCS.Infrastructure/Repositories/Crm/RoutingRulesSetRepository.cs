using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Domain.Models.Crm;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class RoutingRulesSetRepository : CrmRepository<RoutingRulesSet>
    {
        public RoutingRulesSetRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => RoutingRulesSet.EntityLogicalName;

        public List<RoutingRulesSet> GetByEntityName(string name) 
        {
            var query = new QueryExpression(RoutingRulesSet.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(RoutingRulesSet.Metadata.EntityName, 
                                          RoutingRulesSet.Metadata.Name)
            };

            query.Criteria.AddCondition(RoutingRulesSet.Metadata.EntityName, ConditionOperator.Equal, name);
            query.Criteria.AddCondition(EntityCommon.StateCode, ConditionOperator.Equal, 0);
            var result = Get(query);

            return result;
        }
    }
}