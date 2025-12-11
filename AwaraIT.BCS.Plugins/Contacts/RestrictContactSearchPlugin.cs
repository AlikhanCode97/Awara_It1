using AwaraIT.BCS.Application.Features.Contacts;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace AwaraIT.BCS.Plugins.Contacts
{
    public class RestrictContactSearchPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var organizationService = serviceFactory.CreateOrganizationService(context.UserId);

            if (!context.InputParameters.Contains("Query") || !(context.InputParameters["Query"] is QueryBase query))
                return;

            var res = new RestrictContactSearchService(organizationService).Execute(query, context.UserId);
            if (res.IsSuccess)
                context.InputParameters["Query"] = res.Value;
        }
    }
}
