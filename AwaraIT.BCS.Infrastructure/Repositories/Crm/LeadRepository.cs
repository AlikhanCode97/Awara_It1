using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class LeadRepository : CrmRepository<Lead>
    {
        public LeadRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => Lead.EntityLogicalName;

        public Lead GetById(Guid id)
        {
            var res = Get(id, true);
            return res;
        }
    }
}
