using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;

namespace AwaraIT.BCS.Application.Features.Leads
{
    public class DuplicateCheckerRfCRMService
    {
        private readonly Logger _logger;
        private readonly SynchronizationWithExternalSystemTaskRepository _synchronizationWithExternalSystemTaskRepository;

        public DuplicateCheckerRfCRMService(IOrganizationService service)
        {
            _logger = new Logger(service);
            _synchronizationWithExternalSystemTaskRepository = new SynchronizationWithExternalSystemTaskRepository(service);
        }

        public Result<Core.Void> Execute(Guid leadId)
        {
            try
            {
                _synchronizationWithExternalSystemTaskRepository.Create(new SynchronizationWithExternalSystemTask
                {
                    Name = $"Check duplicate RF CRM Task: {DateTime.UtcNow:s}",
                    Topic = Constants.TopicDuplicateCheckerRfCRM,
                    EntityName = Lead.EntityLogicalName,
                    EntityId = leadId.ToString(),
                    StatusCode = SynchronizationWithExternalSystemTask.Metadata.StatusCode.Pending
                });

                return Result<Core.Void>.Success();
            }
            catch (Exception ex)
            {
                _logger.ERROR("Chech duplicate RF CRM", ex.ToString(), Lead.EntityLogicalName, leadId);
                return Result<Core.Void>.Failure(ex.Message);
            }
        }
    }
}
