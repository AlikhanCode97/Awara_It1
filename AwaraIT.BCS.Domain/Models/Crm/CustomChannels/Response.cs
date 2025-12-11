using System;
using System.Collections.Generic;

namespace AwaraIT.BCS.Domain.Models.Crm.CustomChannels
{
    public class Response
    {
        public Guid ChannelDefinitionId { get; set; }
        public string RequestId { get; set; }
        public string MessageId { get; set; }
        public string Status { get; set; }
        public Dictionary<string, object> StatusDetails { get; set; }
    }
}
