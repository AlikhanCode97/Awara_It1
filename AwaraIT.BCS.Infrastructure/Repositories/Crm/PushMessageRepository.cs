using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class PushMessageRepository : CrmRepository<PushMessage>
    {
        public PushMessageRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => PushMessage.EntityLogicalName;
    }
}
