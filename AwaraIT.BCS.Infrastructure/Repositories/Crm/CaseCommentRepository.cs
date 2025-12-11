using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class CaseCommentRepository : CrmRepository<CaseComment>
    {
        public CaseCommentRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => CaseComment.EntityLogicalName;
    }
}
