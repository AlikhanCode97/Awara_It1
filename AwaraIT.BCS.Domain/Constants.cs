namespace AwaraIT.BCS.Domain
{
    public static class Constants
    {
        /// Global
        public const string DateFormat = "dd.MM.yyyy";

        /// CRM
        public const int MaxPageSize = 5000;
        public const string SysAdminUserRoleName = "System Administrator";
        public const string JiraProjectsEnvironmentVariableName = "awr_jira_projects";
        public const string ChannelContractPayload = "payload";
        public const string ChannelContractResponse = "response";
        public const string RestrictContactTargetRolesEnvironmentVariableName = "awr_RestrictContactTargetRoles";

        /// Topics
        public const string TopicDuplicateCheckerRfCRM = "DuplicateCheckerRfCRM";
        public const string TopicSendCaseToJira = "SendCaseToJira";    
        public const string TopicSendCaseCommentToJira = "SendCaseCommentToJira";
    }
}
