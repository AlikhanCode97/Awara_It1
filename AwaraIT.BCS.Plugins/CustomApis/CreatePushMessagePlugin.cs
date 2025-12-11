using AwaraIT.BCS.Application.Features.CustomApis;
using AwaraIT.BCS.Domain;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Extensions;
using System;

namespace AwaraIT.BCS.Plugins.CustomApis
{
    public class CreatePushMessagePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.Get<IPluginExecutionContext>();
            var service = serviceProvider.Get<IOrganizationServiceFactory>().CreateOrganizationService(null);

            var payloadString = (string)context.InputParameters[Constants.ChannelContractPayload];

            var response = new CreatePushMessageService(service).Execute(payloadString);
            context.OutputParameters[Constants.ChannelContractResponse] = response;
        }
    }
}
