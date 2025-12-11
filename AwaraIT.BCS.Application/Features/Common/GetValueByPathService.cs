using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using System.Linq;
using System;

namespace AwaraIT.BCS.Application.Features.Common
{
    public class GetValueByPathService
    {
        private EntityReference _currentEntityRef { get; set; }

        private IOrganizationService _service { get; set; }

        private string _path { get; set; }

        public GetValueByPathService(IOrganizationService service, EntityReference entRef, string path)
        {
            _service = service;
            _currentEntityRef = entRef;
            _path = path;
        }

        public EntityReference GetLastEntityReference() 
        {
            var fieldsList = _path.Split('.');
            if (fieldsList.Length == 0) 
                return null;

            var currentEntity = RetriveIfExists(_currentEntityRef.LogicalName, _currentEntityRef.Id, new ColumnSet(fieldsList[0]));
            var currentRef = GetAttributeValue<EntityReference>(currentEntity, fieldsList[0]);

            if (fieldsList.Length > 1)
            {
                foreach (var field in fieldsList.Skip(1)) 
                {
                    if (currentRef != null)
                    {
                        currentEntity = RetriveIfExists(currentRef.LogicalName, currentRef.Id, new ColumnSet(field));
                        currentRef = GetAttributeValue<EntityReference>(currentEntity, field);
                    }
                    else
                        return null;
                }
            }

            return currentRef;
        }

        private T GetAttributeValue<T>(Entity entity, string attributeName) 
        {
            if (entity == null 
                || string.IsNullOrEmpty(attributeName)
                || !entity.Contains(attributeName))
                return default(T);

            return entity.GetAttributeValue<T>(attributeName);
        }

        private Entity RetriveIfExists(string entityName, Guid Id, ColumnSet columns)
        {
            if (string.IsNullOrEmpty(entityName) || Id == Guid.Empty)
                return null;

            try 
            {
                return _service.Retrieve(entityName, Id, columns);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (ex.Detail.ErrorCode == -2147220969) //ObjectDoesNotExist
                    return null;

                throw;
            }
        }
    }
}