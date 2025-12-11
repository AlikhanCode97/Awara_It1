using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class SynchronizationWithExternalSystemTaskRepository : CrmRepository<SynchronizationWithExternalSystemTask>
    {
        public SynchronizationWithExternalSystemTaskRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => SynchronizationWithExternalSystemTask.EntityLogicalName;
    }
}
