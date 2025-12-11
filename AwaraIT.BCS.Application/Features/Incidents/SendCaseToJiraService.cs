using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Domain.Models.Crm.EnvironmentVariables;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace AwaraIT.BCS.Application.Features.Incidents
{
    public class SendCaseToJiraService
    {
        private readonly Logger _logger;
        private readonly SynchronizationWithExternalSystemTaskRepository _synchronizationWithExternalSystemTaskRepository;
        private readonly IncidentRepository _incidentRepository;
        private readonly Cache _cache;

        public SendCaseToJiraService(IOrganizationService service)
        {
            _logger = new Logger(service);
            _synchronizationWithExternalSystemTaskRepository = new SynchronizationWithExternalSystemTaskRepository(service);
            _incidentRepository = new IncidentRepository(service);
            _cache = new Cache(service);
        }

        public Result<Core.Void> Execute(Guid incidentId, string projectName)
        {
            try
            {
                if (string.IsNullOrEmpty(projectName))
                {
                    _logger.ERROR("Send Case To Jira", "Project name is null or empty.", Incident.EntityLogicalName, incidentId);
                    return Result<Core.Void>.Failure("Project name is null or empty.");
                }

                var environmentVariables = _cache.GetEnvironmentVariables();
                if (!environmentVariables.TryGetValue(Constants.JiraProjectsEnvironmentVariableName, out var jiraProjectsStringObj))
                {
                    _logger.ERROR("Send Case To Jira", "Jira projects environment variable not found.", Incident.EntityLogicalName, incidentId);
                    return Result<Core.Void>.Failure("Jira projects environment variable not found.");
                }

                var jiraProjects = JsonSerializer.Deserialize<ProjectJira[]>(jiraProjectsStringObj);
                var projectJira = jiraProjects.FirstOrDefault(a => projectName.Equals(a.value, StringComparison.OrdinalIgnoreCase));
                if (projectJira == null)
                {
                    _logger.ERROR("Send Case To Jira", $"Project '{projectName}' not found in Jira projects.", Incident.EntityLogicalName, incidentId);
                    return Result<Core.Void>.Failure($"Project '{projectName}' not found in Jira projects.");
                }

                _synchronizationWithExternalSystemTaskRepository.Create(new SynchronizationWithExternalSystemTask
                {
                    Name = $"Jira Case Sync Task: {DateTime.UtcNow:s}",
                    Topic = Constants.TopicSendCaseToJira,
                    EntityName = Incident.EntityLogicalName,
                    EntityId = incidentId.ToString(),
                    StatusCode = SynchronizationWithExternalSystemTask.Metadata.StatusCode.Pending,
                    Parameters = JsonSerializer.Serialize(projectJira)
                });

                _incidentRepository.Update(new Incident
                {
                    Id = incidentId,
                    StatusCode = Incident.Metadata.StatusCode.SentTo2ndline
                });

                return Result<Core.Void>.Success();
            }
            catch (Exception ex)
            {
                _logger.ERROR("Send Case To Jira", ex.ToString(), Incident.EntityLogicalName, incidentId);
                return Result<Core.Void>.Failure(ex.Message);
            }
        }
    }
}
