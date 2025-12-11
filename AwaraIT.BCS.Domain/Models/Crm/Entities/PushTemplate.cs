using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class PushTemplate : BaseEntity
    {
        public PushTemplate() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string Name = "awr_name";
        }

        public const string EntityLogicalName = "awr_pushtemplate";

        public string Name
        {
            get { return GetAttributeValue<string>(Metadata.Name); }
            set { Attributes[Metadata.Name] = value; }
        }
    }
}
