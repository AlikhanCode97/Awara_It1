using System;

namespace AwaraIT.BCS.Domain.Models.Crm.CustomChannels
{
    public class PushPayload
    {
        public Guid ChannelDefinitionId { get; set; }
        public string RequestId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public PushPayloadMessage Message { get; set; }
        public MarketingAppContext MarketingAppContext { get; set; }
    }

    public class PushPayloadMessage
    {
        public Guid? template { get; set; }
        public string text { get; set; }
    }
}
