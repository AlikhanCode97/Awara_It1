using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class IncidentRepository : CrmRepository<Incident>
    {
        public IncidentRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => Incident.EntityLogicalName;

        public Incident GetById(Guid id)
        {
            var res = Get(id, Incident.Metadata.ExternalId, Incident.Metadata.JiraProject);
            return res;
        }
    }
}
