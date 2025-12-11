using System;

namespace AwaraIT.BCS.Domain.Models.Crm.CustomChannels
{
    public class SMSPayload
    {
        public Guid ChannelDefinitionId { get; set; }
        public string RequestId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public SMSPayloadMessage Message { get; set; }
        public MarketingAppContext MarketingAppContext { get; set; }
    }

    public class SMSPayloadMessage
    {
        public string title { get; set; }
        public string text { get; set; }
    }
}
