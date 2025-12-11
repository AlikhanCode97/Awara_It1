using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{

    [EntityLogicalName(EntityLogicalName)]
    public class SystemUserRole : BaseEntity
    {
        public SystemUserRole() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string SystemUserId = "systemuserid";
            public const string RoleId = "roleid";
        }

        public const string EntityLogicalName = "systemuserroles";
    }
}
