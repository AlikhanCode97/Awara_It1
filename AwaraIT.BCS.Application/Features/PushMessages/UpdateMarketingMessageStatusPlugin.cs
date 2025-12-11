using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels.Enums;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Application.Features.PushMessages
{
    public class UpdateMarketingMessageStatusService
    {
        private readonly Logger _logger;
        private readonly MarketingRepository _marketingRepository;

        public UpdateMarketingMessageStatusService(IOrganizationService service)
        {
            _logger = new Logger(service);
            _marketingRepository = new MarketingRepository(service);
        }

        public void Execute(PushMessage messagePushTarget, PushMessage messagePushTargetImg, Guid organizationId)
        {
            try
            {
                var messageStatus = ConvertStatuscodeToMessageStatus(messagePushTarget.StatusCode);
                if (messageStatus == null)
                    return;

                if (string.IsNullOrEmpty(messagePushTargetImg.MarketingId) ||
                    string.IsNullOrEmpty(messagePushTargetImg.ChannelDefinitionId) ||
                    string.IsNullOrEmpty(messagePushTargetImg.MarketingSender))
                    return;

                _marketingRepository.UpdateMarketingMessageStatus(messagePushTarget.Id,
                    organizationId, messagePushTargetImg.ChannelDefinitionId, messagePushTargetImg.MarketingId,
                    messagePushTargetImg.MarketingSender, messageStatus.Value);
            }
            catch (Exception ex)
            {
                _logger.ERROR("Plugin UpdateMarketingMessageStatusPlugin. Error update status",
                    $"Exception: {ex}", messagePushTarget.LogicalName, messagePushTarget.Id);
            }
        }

        private MessageStatus? ConvertStatuscodeToMessageStatus(PushMessage.Metadata.StatusCode? statusCode)
        {
            if (statusCode == null)
                return null;

            switch (statusCode)
            {
                case PushMessage.Metadata.StatusCode.Delivered: return MessageStatus.Delivered;
                case PushMessage.Metadata.StatusCode.NotDelivered: return MessageStatus.NotDelivered;
                case PushMessage.Metadata.StatusCode.NotSent: return MessageStatus.NotDelivered;
                default: return null;
            }
        }
    }
}
