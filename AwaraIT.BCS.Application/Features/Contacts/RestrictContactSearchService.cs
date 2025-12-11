using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AwaraIT.BCS.Application.Features.Contacts
{
    public class RestrictContactSearchService
    {
        private const string ReturnTotalRecordCountPattern = @"\sreturntotalrecordcount=[""']true[""']";
        private const string PageCountPattern = @"\s(page|count)=[""']\d+[""']";
        private const string TopPattern = @"\s(top)=[""']\d+[""']";

        private readonly Logger _logger;
        private readonly RoleRepository _roleRepository;
        private readonly Cache _cache;

        public RestrictContactSearchService(IOrganizationService service)
        {
            _logger = new Logger(service);
            _roleRepository = new RoleRepository(service);
            _cache = new Cache(service);
        }

        public Result<QueryBase> Execute(QueryBase query, Guid userId)
        {
            try
            {
                var roles = _roleRepository.GetUserRoles(userId);
                if (roles.Any(role => role.Name == Constants.SysAdminUserRoleName))
                    return Result<QueryBase>.Success(query);

                var environmentVariables = _cache.GetEnvironmentVariables();
                if (!environmentVariables.TryGetValue(Constants.RestrictContactTargetRolesEnvironmentVariableName, 
                    out var restrictContactTargetRolesEnvironmentVariable) || string.IsNullOrEmpty(restrictContactTargetRolesEnvironmentVariable))
                {
                    return Result<QueryBase>.Failure("Couldn't find awr_RestrictContactTargetRoles or is empty");
                }
                var restrictContactTargetRoles = restrictContactTargetRolesEnvironmentVariable.Split(',').ToList();

                if (!roles.Any(a => restrictContactTargetRoles.Contains(a.Name.Trim())))
                    return Result<QueryBase>.Success(query);

                switch (query)
                {
                    case QueryExpression queryExpression:
                        queryExpression.TopCount = 1;
                        queryExpression.PageInfo = null;
                        return Result<QueryBase>.Success(queryExpression);

                    case FetchExpression fetchExpression:
                        var fetchXml = fetchExpression.Query;
                        if (!fetchXml.Contains("top="))
                        {
                            fetchXml = Regex.Replace(fetchXml, ReturnTotalRecordCountPattern, "");
                            fetchXml = Regex.Replace(fetchXml, PageCountPattern, "");

                            var insertPosition = fetchXml.IndexOf("<fetch", StringComparison.Ordinal);
                            if (insertPosition >= 0)
                            {
                                insertPosition += "<fetch".Length;
                                fetchXml = fetchXml.Insert(insertPosition, " top='1'");
                                fetchExpression.Query = fetchXml;
                            }
                        }
                        else
                        {
                            fetchExpression.Query = Regex.Replace(fetchXml, TopPattern, " top='1'");
                        }

                        return Result<QueryBase>.Success(fetchExpression);

                    default:
                        return Result<QueryBase>.Success(query);
                }
            }
            catch (Exception ex)
            {
                _logger.ERROR("RestrictContactSearch Plugin", ex.ToString());
                return Result<QueryBase>.Failure(ex.Message);
            }
        }
    }
}
