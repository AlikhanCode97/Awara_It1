using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class RoutingElement : BaseActionEntity
    {
        public RoutingElement() : base(EntityLogicalName) { }

        public const string EntityLogicalName = "awr_routingelement";

        public static class Metadata
        {
            public const string RoutingRuleElementId = "awr_routingruleelementid";
            public const string RoutingRulesSetId = "awr_routingrulessetid";
            public const string SystemUserId = "awr_systemuserid";
            public const string TeamId = "awr_teamid";
        }

        public EntityReference RoutingRuleElementId
        {
            get { return GetAttributeValue<EntityReference>(Metadata.RoutingRuleElementId); }
            set { Attributes[Metadata.RoutingRuleElementId] = value; }
        }

        public EntityReference RoutingRulesSetId
        {
            get { return GetAttributeValue<EntityReference>(Metadata.RoutingRulesSetId); }
            set { Attributes[Metadata.RoutingRulesSetId] = value; }
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
    }
}