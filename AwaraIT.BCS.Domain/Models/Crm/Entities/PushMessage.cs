using AwaraIT.BCS.Domain.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class PushMessage : BaseActionEntity
    {
        public PushMessage() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string ChannelDefinitionId = "awr_channeldefinitionid";
            public const string ErrorMessage = "awr_errormessage";
            public const string MarketingId = "awr_marketingid";
            public const string MarketingSender = "awr_marketingsender";
            public const string PushTemplate = "awr_pushtemplate";

            public enum StatusCode
            {
                Pending = 1,
                Sent = 187760001,
                NotSent = 187760002,
                Delivered = 187760003,
                NotDelivered = 187760004,
            }
        }

        public const string EntityLogicalName = "awr_pushmessage";

        public Metadata.StatusCode? StatusCode
        {
            get { return (Metadata.StatusCode?)GetAttributeValue<OptionSetValue>(EntityCommon.StatusCode)?.Value; }
            set { Attributes[EntityCommon.StatusCode] = value != null ? new OptionSetValue((int)value.Value) : null; }
        }

        public string ChannelDefinitionId
        {
            get { return GetAttributeValue<string>(Metadata.ChannelDefinitionId); }
            set { Attributes[Metadata.ChannelDefinitionId] = value; }
        }

        public string ErrorMessage
        {
            get { return GetAttributeValue<string>(Metadata.ErrorMessage); }
            set { Attributes[Metadata.ErrorMessage] = value.Crop(2000); }
        }

        public string MarketingId
        {
            get { return GetAttributeValue<string>(Metadata.MarketingId); }
            set { Attributes[Metadata.MarketingId] = value; }
        }

        public string MarketingSender
        {
            get { return GetAttributeValue<string>(Metadata.MarketingSender); }
            set { Attributes[Metadata.MarketingSender] = value; }
        }

        public EntityReference PushTemplate
        {
            get { return GetAttributeValue<EntityReference>(Metadata.PushTemplate); }
            set { Attributes[Metadata.PushTemplate] = value; }
        }
    }
}
