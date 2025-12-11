using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class SynchronizationWithExternalSystemTask : BaseEntity
    {
        public SynchronizationWithExternalSystemTask() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string SynchronizationWithExternalSystemTaskId = "awr_synchronizationwithexternalsystemtaskid";
            public const string EntityId = "awr_entityid";
            public const string EntityName = "awr_entityname";
            public const string ErrorMessage = "awr_errormessage";
            public const string Name = "awr_name";
            public const string SyncDate = "awr_syncdate";
            public const string Parameters = "awr_parameters";
            public const string Topic = "awr_topic";

            public enum StatusCode
            {
                Pending = 1,
                Success = 752440001,
                Error = 752440002
            }
        }

        public const string EntityLogicalName = "awr_synchronizationwithexternalsystemtask";

        public Metadata.StatusCode? StatusCode
        {
            get { return (Metadata.StatusCode?)GetAttributeValue<OptionSetValue>(EntityCommon.StatusCode)?.Value; }
            set { Attributes[EntityCommon.StatusCode] = value != null ? new OptionSetValue((int)value.Value) : null; }
        }

        public string ErrorMessage
        {
            get { return GetAttributeValue<string>(Metadata.ErrorMessage); }
            set { Attributes[Metadata.ErrorMessage] = value; }
        }

        public string EntityId
        {
            get { return GetAttributeValue<string>(Metadata.EntityId); }
            set { Attributes[Metadata.EntityId] = value; }
        }

        public string EntityName
        {
            get { return GetAttributeValue<string>(Metadata.EntityName); }
            set { Attributes[Metadata.EntityName] = value; }
        }

        public string Name
        {
            get { return GetAttributeValue<string>(Metadata.Name); }
            set { Attributes[Metadata.Name] = value; }
        }

        public DateTime? SyncDate
        {
            get { return GetAttributeValue<DateTime?>(Metadata.SyncDate); }
            set { Attributes[Metadata.SyncDate] = value; }
        }

        public string Parameters
        {
            get { return GetAttributeValue<string>(Metadata.Parameters); }
            set { Attributes[Metadata.Parameters] = value; }
        }

        public string Topic
        {
            get { return GetAttributeValue<string>(Metadata.Topic); }
            set { Attributes[Metadata.Topic] = value; }
        }
    }
}
