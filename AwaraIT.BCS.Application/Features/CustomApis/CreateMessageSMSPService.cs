using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels.Enums;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Application.Features.CustomApis
{
    public class CreateMessageSMSPService
    {
        private readonly Logger _logger;
        private readonly SMSMessageRepository _smsMessageRepository;

        public CreateMessageSMSPService(IOrganizationService service)
        {
            _logger = new Logger(service);
            _smsMessageRepository = new SMSMessageRepository(service);
        }

        public string Execute(string payloadString)
        {
            var smsPayload = JsonSerializer.Deserialize<SMSPayload>(payloadString);

            try
            {

                if (string.IsNullOrEmpty(smsPayload.Message.text) || string.IsNullOrEmpty(smsPayload.Message.title))
                    throw new InvalidPluginExecutionException("Invalid input parameters");

                var messageSMSId = CreateMessageSms(smsPayload);
                var response = GetResponse(smsPayload, MessageStatus.Sent, messageSMSId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ERROR("Service CreateMessageSMSPService. Error creating SMS message", $"Exception: {ex}, payloadString: {payloadString}");
                var messageId = Guid.NewGuid();
                var response = GetResponse(smsPayload, MessageStatus.NotSent, messageId);
                return response;
            }
        }

        private Guid CreateMessageSms(SMSPayload smsPayload)
        {
            var regardingObjectId = new EntityReference(smsPayload.MarketingAppContext.UserEntityType,
                smsPayload.MarketingAppContext.UserId);

            var messageSMSId = _smsMessageRepository.Create(new SMSMessage
            {
                Subject = smsPayload.Message.title,
                Description = smsPayload.Message.text,
                RegardingObjectId = regardingObjectId,
                StatusCode = SMSMessage.Metadata.StatusCode.Pending,
                MarketingId = smsPayload.RequestId,
                ChannelDefinitionId = smsPayload.ChannelDefinitionId.ToString(),
                MarketingSender = smsPayload.From
            });

            return messageSMSId;
        }

        private string GetResponse(SMSPayload smsPayload, MessageStatus messageStatus,
            Guid messageSMSId)
        {
            var responseObject = new Response()
            {
                ChannelDefinitionId = smsPayload.ChannelDefinitionId,
                MessageId = messageSMSId.ToString(),
                RequestId = smsPayload.RequestId,
                Status = messageStatus.ToString(),
                StatusDetails = null
            };

            return JsonSerializer.Serialize(responseObject);
        }
    }
}
