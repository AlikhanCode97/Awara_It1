using System;
using System.Collections.Generic;

namespace AwaraIT.BCS.Domain.Models.Crm.CustomChannels
{
    public class DeliveryReport
    {
        public Guid ChannelDefinitionId { get; set; }
        public string RequestId { get; set; }
        public string MessageId { get; set; }
        public string Status { get; set; }
        public IDictionary<string, object> StatusDetails { get; set; }
        public string From { get; set; }
        public string OrganizationId { get; set; }
    }
}
