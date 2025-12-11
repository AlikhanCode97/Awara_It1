using System;

namespace AwaraIT.BCS.Domain.Models.Crm.CustomChannels
{
    public class MarketingAppContext
    {
        public Guid UserId { get; set; }
        public string UserEntityType { get; set; }
        public string CustomerJourneyId { get; set; }
    }
}
