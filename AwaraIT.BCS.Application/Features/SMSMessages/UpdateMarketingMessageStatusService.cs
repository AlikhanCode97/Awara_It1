using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels.Enums;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Application.Features.SMSMessages
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

        public void Execute(SMSMessage messageSMSTarget, SMSMessage messageSMSTargetImg, Guid organizationId)
        {
            try
            {
                var messageStatus = ConvertStatuscodeToMessageStatus(messageSMSTarget.StatusCode);
                if (messageStatus == null)
                    return;

                if (string.IsNullOrEmpty(messageSMSTargetImg.MarketingId) ||
                    string.IsNullOrEmpty(messageSMSTargetImg.ChannelDefinitionId) ||
                    string.IsNullOrEmpty(messageSMSTargetImg.MarketingSender))
                    return;

                _marketingRepository.UpdateMarketingMessageStatus(messageSMSTarget.Id,
                    organizationId, messageSMSTargetImg.ChannelDefinitionId, messageSMSTargetImg.MarketingId,
                    messageSMSTargetImg.MarketingSender, messageStatus.Value);
            }
            catch (Exception ex)
            {
                _logger.ERROR("Plugin UpdateMarketingMessageStatusPlugin. Error update status",
                    $"Exception: {ex}", messageSMSTarget.LogicalName, messageSMSTarget.Id);
            }
        }

        private MessageStatus? ConvertStatuscodeToMessageStatus(SMSMessage.Metadata.StatusCode? statusCode)
        {
            if (statusCode == null)
                return null;

            switch (statusCode)
            {
                case SMSMessage.Metadata.StatusCode.Success: return MessageStatus.Delivered;
                case SMSMessage.Metadata.StatusCode.Error: return MessageStatus.NotDelivered;
                default: return null;
            }
        }
    }
}
