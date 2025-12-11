using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Domain.Models.Crm.EnvironmentVariables;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace AwaraIT.BCS.Application.Features.CaseComments
{
    public class SendCaseCommentToJiraService
    {
        private readonly Logger _logger;
        private readonly SynchronizationWithExternalSystemTaskRepository _synchronizationWithExternalSystemTaskRepository;
        private readonly IncidentRepository _incidentRepository;
        private readonly Cache _cache;

        public SendCaseCommentToJiraService(IOrganizationService service)
        {
            _logger = new Logger(service);
            _synchronizationWithExternalSystemTaskRepository = new SynchronizationWithExternalSystemTaskRepository(service);
            _incidentRepository = new IncidentRepository(service);
            _cache = new Cache(service);
        }

        public Result<Core.Void> Execute(CaseComment caseComment)
        {
            try
            {
                if (!caseComment.SendsecondLine.GetValueOrDefault() ||
                    !caseComment.Direction.GetValueOrDefault() ||
                    !string.IsNullOrEmpty(caseComment.ExternalId))
                    return Result<Core.Void>.Success();

                if (caseComment.RegardingObjectId == null)
                    throw new InvalidPluginExecutionException("RegardingObjectId is null");

                if (!caseComment.RegardingObjectId.LogicalName.Equals(Incident.EntityLogicalName))
                    return Result<Core.Void>.Success();

                var incident = _incidentRepository.GetById(caseComment.RegardingObjectId.Id);
                if (string.IsNullOrEmpty(incident.ExternalId) || string.IsNullOrEmpty(incident.JiraProject))
                    return Result<Core.Void>.Success();

                var environmentVariables = _cache.GetEnvironmentVariables();
                if (!environmentVariables.TryGetValue(Constants.JiraProjectsEnvironmentVariableName, out var jiraProjectsStringObj))
                {
                    _logger.ERROR("Send Case Comment To Jira", "Jira projects environment variable not found.", CaseComment.EntityLogicalName,
                        caseComment.Id);
                    return Result<Core.Void>.Failure("Jira projects environment variable not found.");
                }

                var jiraProjects = JsonSerializer.Deserialize<ProjectJira[]>(jiraProjectsStringObj);
                var projectJira = jiraProjects.FirstOrDefault(a => incident.JiraProject.Equals(a.value, StringComparison.OrdinalIgnoreCase));
                if (projectJira == null)
                {
                    _logger.ERROR("Send Case Comment To Jira", $"Project '{incident.JiraProject}' not found in Jira projects.", CaseComment.EntityLogicalName, 
                        caseComment.Id);
                    return Result<Core.Void>.Failure($"Project '{incident.JiraProject}' not found in Jira projects.");
                }
                if (!projectJira.syncComments)
                    return Result<Core.Void>.Success();

                _synchronizationWithExternalSystemTaskRepository.Create(new SynchronizationWithExternalSystemTask
                {
                    Name = $"Jira Comment Sync Task: {DateTime.UtcNow:s}",
                    Topic = Constants.TopicSendCaseCommentToJira,
                    EntityName = CaseComment.EntityLogicalName,
                    EntityId = caseComment.Id.ToString(),
                    StatusCode = SynchronizationWithExternalSystemTask.Metadata.StatusCode.Pending
                });

                return Result<Core.Void>.Success();
            }
            catch (Exception ex)
            {
                _logger.ERROR("Send Case Comment To Jira", ex.ToString(), CaseComment.EntityLogicalName, caseComment.Id);
                return Result<Core.Void>.Failure(ex.Message);
            }
        }
    }
}
