using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Infrastructure.Common
{
    public static class OrganizationServiceWrapper
    {
        public static bool CheckIfEntityIsActivityByName(this IOrganizationService service, string logicalName) 
        {
            var response = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = logicalName
            });

            return (bool)response.EntityMetadata.IsActivity;
        }

        public static (string name, bool isActivity) GetEntityPrimaryField(this IOrganizationService service, string logicalName)
        {
            if (service.CheckIfEntityIsActivityByName(logicalName))
                return ("activityid", true);

            return (logicalName + "id", false);
        }

        public static void AssignTo(this IOrganizationService service, EntityReference assignedEntRef, EntityReference ownerRef) 
        {
            var entity = new Entity(assignedEntRef.LogicalName, assignedEntRef.Id);
            entity["ownerid"] = ownerRef;

            service.Update(entity);
        }
    }
}