using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class RoutingRuleElement : BaseEntity
    {
        public RoutingRuleElement() : base(EntityLogicalName) { }

        public const string EntityLogicalName = "awr_routingruleelement";

        public static class Metadata
        {
            public const string Name = "awr_name";
            public const string RoutingRulesSetId = "awr_routingrulessetid";
            public const string AssignAt = "awr_assignat";
            public const string SystemUserId = "awr_systemuserid";
            public const string TeamId = "awr_teamid";
            public const string Priority = "awr_priority";
            public const string EntityName = "awr_entityname";
            public const string FetchFilter = "awr_fetchfilter";
            public const string Path = "awr_path";

            public enum AssignAtEnum
            {
                User = 752440000,
                Team = 752440001,
                Custom = 752440002
            }
        }

        public string Name
        {
            get { return GetAttributeValue<string>(Metadata.Name); }
            set { Attributes[Metadata.Name] = value; }
        }

        public EntityReference RoutingRulesSetId
        {
            get { return GetAttributeValue<EntityReference>(Metadata.RoutingRulesSetId); }
            set { Attributes[Metadata.RoutingRulesSetId] = value; }
        }

        public Metadata.AssignAtEnum? AssignAt
        {
            get { return (Metadata.AssignAtEnum?)GetAttributeValue<OptionSetValue>(Metadata.AssignAt)?.Value; }
            set { Attributes[Metadata.AssignAt] = value != null ? new OptionSetValue((int)value.Value) : null; }
        }

        public EntityReference SystemUserId
        {
            get { return GetAttributeValue<EntityReference>(Metadata.SystemUserId); }
            set { Attributes[Metadata.SystemUserId] = value; }
        }

        public EntityReference TeamId
        {
            get { return GetAttributeValue<EntityReference>(Metadata.TeamId); }
            set { Attributes[Metadata.TeamId] = value; }
        }

        public int? Priority
        {
            get { return GetAttributeValue<int>(Metadata.Priority); }
            set { Attributes[Metadata.Priority] = value; }
        }

        public string EntityName
        {
            get { return GetAttributeValue<string>(Metadata.EntityName); }
            set { Attributes[Metadata.EntityName] = value; }
        }

        public string FetchFilter
        {
            get { return GetAttributeValue<string>(Metadata.FetchFilter); }
            set { Attributes[Metadata.FetchFilter] = value; }
        }

        public string Path
        {
            get { return GetAttributeValue<string>(Metadata.Path); }
            set { Attributes[Metadata.Path] = value; }
        }
    }
}