using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace AwaraIT.BCS.Domain.Models.Crm.EnvironmentVariables {

    public class CrmEnvironmentVariables {
        public static string GetSharepointPathUrl(IOrganizationService service, string schemaName)
        {
            var definitionQuery = new QueryExpression("environmentvariabledefinition")
            {
                ColumnSet = new ColumnSet("defaultvalue"),
            };
            definitionQuery.Criteria.AddCondition("schemaname", ConditionOperator.Equal, schemaName);

            var definition = service.RetrieveMultiple(definitionQuery).Entities.FirstOrDefault();
            if (definition == null)
                throw new InvalidPluginExecutionException($"Environment variable '{schemaName}' not found.");

            string defaultValue = definition.GetAttributeValue<string>("defaultvalue");

            var valueQuery = new QueryExpression("environmentvariablevalue")
            {
                ColumnSet = new ColumnSet("value"),
            };
            valueQuery.Criteria.AddCondition("environmentvariabledefinitionid", ConditionOperator.Equal, definition.Id);

            var value = service.RetrieveMultiple(valueQuery).Entities.FirstOrDefault();
            string runtimeValue = value?.GetAttributeValue<string>("value");

            return runtimeValue ?? defaultValue;
        }

    }
}