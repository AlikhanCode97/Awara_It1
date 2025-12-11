using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels;
using AwaraIT.BCS.Domain.Models.Crm.CustomChannels.Enums;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Application.Features.CustomApis
{
    public class CreatePushMessageService
    {
        private readonly Logger _logger;
        private readonly PushMessageRepository _pushMessageRepository;

        public CreatePushMessageService(IOrganizationService service)
        {
            _logger = new Logger(service);
            _pushMessageRepository = new PushMessageRepository(service);
        }

        public string Execute(string payloadString)
        {
            var pushPayload = JsonSerializer.Deserialize<PushPayload>(payloadString);

            try
            {

                if (string.IsNullOrEmpty(pushPayload.Message.text) || pushPayload.Message.template == null)
                    throw new InvalidPluginExecutionException("Invalid input parameters");

                var messagePushId = CreateMessagePush(pushPayload);
                var response = GetResponse(pushPayload, MessageStatus.Sent, messagePushId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.ERROR("Service CreatePushMessageService. Error creating Push message", $"Exception: {ex}, payloadString: {payloadString}");
                var messageId = Guid.NewGuid();
                var response = GetResponse(pushPayload, MessageStatus.NotSent, messageId);
                return response;
            }
        }

        private Guid CreateMessagePush(PushPayload pushPayload)
        {
            var regardingObjectId = new EntityReference(pushPayload.MarketingAppContext.UserEntityType,
                pushPayload.MarketingAppContext.UserId);

            var messagePushId = _pushMessageRepository.Create(new PushMessage
            {
                Subject = $"Push Message {DateTime.UtcNow}",
                PushTemplate = new EntityReference(PushTemplate.EntityLogicalName, pushPayload.Message.template.Value),
                Description = pushPayload.Message.text,
                RegardingObjectId = regardingObjectId,
                StatusCode = PushMessage.Metadata.StatusCode.Pending,
                MarketingId = pushPayload.RequestId,
                ChannelDefinitionId = pushPayload.ChannelDefinitionId.ToString(),
                MarketingSender = pushPayload.From
            });

            return messagePushId;
        }

        private string GetResponse(PushPayload pushPayload, MessageStatus messageStatus,
            Guid messagePushId)
        {
            var responseObject = new Response()
            {
                ChannelDefinitionId = pushPayload.ChannelDefinitionId,
                MessageId = messagePushId.ToString(),
                RequestId = pushPayload.RequestId,
                Status = messageStatus.ToString(),
                StatusDetails = null
            };

            return JsonSerializer.Serialize(responseObject);
        }
    }
}
