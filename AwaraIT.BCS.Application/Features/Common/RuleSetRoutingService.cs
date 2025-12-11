using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Infrastructure.Common;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System.Linq;
using System;

namespace AwaraIT.BCS.Application.Features.Common
{
    public class RuleSetRoutingService
    {
        private EntityReference _targetEntityRef { get; set; }

        private IOrganizationService _service { get; set; }

        public RuleSetRoutingService(IOrganizationService service, EntityReference entRef)
        {
            _service = service;
            _targetEntityRef = entRef;
        }

        public bool Run()
        {
            var routingRuleElements = new RoutingRuleElementRepository(_service).GetByEntityName(_targetEntityRef.LogicalName);
            if (routingRuleElements == null || !routingRuleElements.Any())
                return false;

            bool recordInFetch;
            var primaryField = _service.GetEntityPrimaryField(_targetEntityRef.LogicalName);
            string[] legalOwners = { "team", "systemuser" };
            var customEnumValue = RoutingRuleElement.Metadata.AssignAtEnum.Custom;

            foreach (var item in routingRuleElements)
            {
                try //fetchXml can be broken
                {
                    recordInFetch = CheckIfIdExistsInFetchData(item.FetchFilter, _targetEntityRef, primaryField.name);
                    if (!recordInFetch 
                        || (item.AssignAt != customEnumValue && item.SystemUserId == null && item.TeamId == null) 
                        || (item.AssignAt == customEnumValue && string.IsNullOrEmpty(item.Path)))
                        continue;

                    EntityReference owner = null;

                    if (item.AssignAt == customEnumValue)
                    {
                        if (string.IsNullOrEmpty(item.Path))
                            return false;

                        owner = new GetValueByPathService(_service, _targetEntityRef, item.Path).GetLastEntityReference();
                        if (owner == null || !legalOwners.Contains(owner.LogicalName))
                            return false;

                        _service.AssignTo(_targetEntityRef, owner);
                    }
                    else
                    {
                        _service.AssignTo(_targetEntityRef, item.SystemUserId ?? item.TeamId);
                        owner = item.SystemUserId ?? item.TeamId;
                    }

                    new RoutingElementRepository(_service).CreateByRoutingRuleElement(_targetEntityRef, item, owner, primaryField.isActivity);

                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error while processing RoutingRuleElement: {item.Id}, fetch: {item.FetchFilter}. Error message: {ex.Message}. Inner error message: {ex.InnerException?.Message}.", ex);
                }
            }

            return false;
        }

        private bool CheckIfIdExistsInFetchData(string fetch, EntityReference entRef, string primaryFieldName)
        {
            if (string.IsNullOrEmpty(fetch)) return false;

            var fetchRequest = new FetchXmlToQueryExpressionRequest
            {
                FetchXml = fetch,
            };

            var fetchResponse = (FetchXmlToQueryExpressionResponse)_service.Execute(fetchRequest);
            var query = fetchResponse.Query;
            query.Criteria.AddCondition(primaryFieldName, ConditionOperator.Equal, entRef.Id.ToString());

            return _service.RetrieveMultiple(query)?.Entities?.Count() > 0;
        }
    }
}