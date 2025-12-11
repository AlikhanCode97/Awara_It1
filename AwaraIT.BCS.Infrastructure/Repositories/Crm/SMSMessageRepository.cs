using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class SMSMessageRepository : CrmRepository<SMSMessage>
    {
        public SMSMessageRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => SMSMessage.EntityLogicalName;
    }
}
