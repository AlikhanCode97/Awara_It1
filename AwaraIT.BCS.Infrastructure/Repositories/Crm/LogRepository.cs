using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class LogRepository : CrmRepository<Log>
    {
        public LogRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => Log.EntityLogicalName;
    }
}
