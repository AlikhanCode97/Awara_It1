using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Incident : BaseEntity
    {
        public Incident() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string IncidentId = "incidentid";
            public const string ExternalId = "awr_externalid";
            public const string JiraProject = "awr_jiraproject";

            public enum StatusCode
            {
                SentTo2ndline = 752440001
            }
        }

        public const string EntityLogicalName = "incident";

        public Metadata.StatusCode? StatusCode
        {
            get { return (Metadata.StatusCode?)GetAttributeValue<OptionSetValue>(EntityCommon.StatusCode)?.Value; }
            set { Attributes[EntityCommon.StatusCode] = value != null ? new OptionSetValue((int)value.Value) : null; }
        }

        public string ExternalId
        {
            get { return GetAttributeValue<string>(Metadata.ExternalId); }
            set { Attributes[Metadata.ExternalId] = value; }
        }

        public string JiraProject
        {
            get { return GetAttributeValue<string>(Metadata.JiraProject); }
            set { Attributes[Metadata.JiraProject] = value; }
        }
    }
}
