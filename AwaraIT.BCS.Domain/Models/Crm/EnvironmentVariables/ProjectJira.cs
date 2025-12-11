using System.Collections.Generic;

namespace AwaraIT.BCS.Domain.Models.Crm.EnvironmentVariables
{
    public class ProjectJira
    {
        public string value { get; set; }
        public string displayName { get; set; }
        public bool useSubjectForSummary { get; set; }
        public bool sendPriority { get; set; } = true;
        public string businessProcess { get; set; }
        public string deviceOperatingSystem { get; set; }
        public string clientID { get; set; }
        public string customerServiceLevel { get; set; }
        public string email { get; set; }
        public string requestSource { get; set; }
        public string dateUserAppeal { get; set; }
        public string themeByClient { get; set; }
        public string attachmentLinkFromClients { get; set; }
        public string attachmentLinkFromExecutor { get; set; }
        public bool syncComments { get; set; } = true;
        public Dictionary<string, string> deviceOperatingSystemValues { get; set; }
        public string fieldsForUpdate { get; set; }
        public string responseToTheClient { get; set; }
        public string issueType { get; set; }
    }
}
