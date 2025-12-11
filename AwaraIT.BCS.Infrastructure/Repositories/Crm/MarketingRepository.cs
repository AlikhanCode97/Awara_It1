using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels.Enums;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class MarketingRepository
    {
        private readonly IOrganizationService _service;

        public MarketingRepository(IOrganizationService service)
        {
            _service = service;
        }

        public void UpdateMarketingMessageStatus(Guid targetEntityId, Guid organizationId, string channelDefinitionId,
            string marketingId, string marketingSender, MessageStatus messageStatus)
        {
            var deliveryReport = new DeliveryReport()
            {
                ChannelDefinitionId = Guid.Parse(channelDefinitionId),
                MessageId = targetEntityId.ToString(),
                RequestId = marketingId,
                Status = messageStatus.ToString(),
                StatusDetails = new Dictionary<string, object>(),
                From = marketingSender,
                OrganizationId = organizationId.ToString()
            };
            var notificatonPayload = JsonSerializer.Serialize(deliveryReport);

            var request = new OrganizationRequest("msdyn_D365ChannelsNotification")
            {
                Parameters =
                {
                    { "notificationPayLoad", notificatonPayload }
                }
            };
            _service.Execute(request);
        }
    }
}
