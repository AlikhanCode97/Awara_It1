using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class CaseComment : BaseActionEntity
    {
        public CaseComment() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string Direction = "awr_direction";
            public const string SendsecondLine = "awr_sendsecondline";
            public const string ExternalId = "awr_externalid";
        }

        public const string EntityLogicalName = "awr_casecomment";

        public bool? Direction
        {
            get { return GetAttributeValue<bool?>(Metadata.Direction); }
            set { Attributes[Metadata.Direction] = value; }
        }

        public bool? SendsecondLine
        {
            get { return GetAttributeValue<bool?>(Metadata.SendsecondLine); }
            set { Attributes[Metadata.SendsecondLine] = value; }
        }

        public string ExternalId
        {
            get { return GetAttributeValue<string>(Metadata.ExternalId); }
            set { Attributes[Metadata.ExternalId] = value; }
        }
    }
}
