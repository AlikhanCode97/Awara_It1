using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class RoutingRulesSet : BaseEntity
    {
        public RoutingRulesSet() : base(EntityLogicalName) { }

        public const string EntityLogicalName = "awr_routingrulesset";

        public static class Metadata
        {
            public const string RoutingRulesSetId = "awr_routingrulessetid";
            public const string EntityName = "awr_entityname";
            public const string Name = "awr_name";
        }

        public string EntityName
        {
            get { return GetAttributeValue<string>(Metadata.EntityName); }
            set { Attributes[Metadata.EntityName] = value; }
        }

        public string Name
        {
            get { return GetAttributeValue<string>(Metadata.Name); }
            set { Attributes[Metadata.Name] = value; }
        }
    }
}