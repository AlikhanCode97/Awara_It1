using AwaraIT.BCS.Domain;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class RoleRepository : CrmRepository<Role>
    {
        public RoleRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => Role.EntityLogicalName;

        public bool IsAdmin(Guid userId)
        {
            var roles = GetUserRoles(userId);

            var isSystemAdmin = roles.Any(role => role.Name == Constants.SysAdminUserRoleName);
            return isSystemAdmin;
        }

        public List<Role> GetUserRoles(Guid userId)
        {
            var query = new QueryExpression(Role.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(Role.Metadata.Name)
            };

            var linkSystemUserRole = query.AddLink(SystemUserRole.EntityLogicalName, Role.Metadata.RoleId, SystemUserRole.Metadata.RoleId);
            var linkSystemUser = linkSystemUserRole.AddLink(SystemUser.EntityLogicalName, SystemUserRole.Metadata.SystemUserId, SystemUser.Metadata.SystemUserId);
            linkSystemUser.LinkCriteria.AddCondition(SystemUserRole.Metadata.SystemUserId, ConditionOperator.Equal, userId);

            var roles = Get(query);
            return roles;
        }
    }
}
