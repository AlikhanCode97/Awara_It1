using AwaraIT.BCS.Domain.Extensions;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.Generic;
using System.Linq;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class EnvironmentVariableDefinitionRepository : CrmRepository<EnvironmentVariableDefinition>
    {
        public EnvironmentVariableDefinitionRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => EnvironmentVariableDefinition.EntityLogicalName;

        public Dictionary<string, string> GetAllWithValue()
        {
            var query = new QueryExpression(EnvironmentVariableDefinition.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(EnvironmentVariableDefinition.Metadata.DefaultValue, EnvironmentVariableDefinition.Metadata.SchemaName)
            };
            var link = query.AddLink(EnvironmentVariableValue.EntityLogicalName, EnvironmentVariableDefinition.Metadata.EnvironmentVariableDefinitionId,
                EnvironmentVariableValue.Metadata.EnvironmentVariableDefinitionId);
            link.Columns = new ColumnSet(EnvironmentVariableValue.Metadata.Value);
            link.EntityAlias = EnvironmentVariableValue.EntityLogicalName;
            var response = Get(query);

            var res = response.GroupBy(a => a.Id)
                .ToDictionary(
                    b => b.First().SchemaName,
                    b => b.First().GetAliasedValue<string>($"{EnvironmentVariableValue.EntityLogicalName}.{EnvironmentVariableValue.Metadata.Value}") ?? b.First().DefaultValue
                );
            return res;
        }

        public string GetValueBySchemaName(string schemaname)
        {
            var query = new QueryExpression(EnvironmentVariableDefinition.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(EnvironmentVariableDefinition.Metadata.SchemaName)
            };

            query.Criteria.AddCondition(EnvironmentVariableDefinition.Metadata.SchemaName, ConditionOperator.Equal, schemaname);

            var link = query.AddLink(EnvironmentVariableValue.EntityLogicalName,
                                     EnvironmentVariableDefinition.Metadata.EnvironmentVariableDefinitionId,
                                     EnvironmentVariableValue.Metadata.EnvironmentVariableDefinitionId);

            link.Columns = new ColumnSet(EnvironmentVariableValue.Metadata.Value);
            link.EntityAlias = EnvironmentVariableValue.EntityLogicalName;

            var response = Get(query)?.FirstOrDefault();
            var envValue = response?.GetAttributeValue<AliasedValue>($"{EnvironmentVariableValue.EntityLogicalName}.{EnvironmentVariableValue.Metadata.Value}")?.Value?.ToString();

            return envValue;
        }
    }
}
