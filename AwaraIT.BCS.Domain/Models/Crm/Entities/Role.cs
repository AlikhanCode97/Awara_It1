using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Role : BaseEntity
    {
        public Role() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string RoleId = "roleid";
            public const string Name = "name";
        }

        public const string EntityLogicalName = "role";

        public string Name
        {
            get { return GetAttributeValue<string>(Metadata.Name); }
            set { Attributes[Metadata.Name] = value; }
        }
    }
}
