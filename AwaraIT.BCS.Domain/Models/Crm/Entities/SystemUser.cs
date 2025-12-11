using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class SystemUser : BaseEntity
    {
        public SystemUser() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string SystemUserId = "systemuserid";
        }

        public const string EntityLogicalName = "systemuser";
    }
}
