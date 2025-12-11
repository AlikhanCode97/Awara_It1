using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Lead : BaseEntity
    {
        public Lead() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string MobilePhone = "mobilephone";
            public const string Email = "emailaddress1";
            public const string ParentContact = "parentcontactid";
        }

        public const string EntityLogicalName = "lead";

        public string MobilePhone
        {
            get { return GetAttributeValue<string>(Metadata.MobilePhone); }
            set { Attributes[Metadata.MobilePhone] = value; }
        }

        public EntityReference ParentContact
        {
            get { return GetAttributeValue<EntityReference>(Metadata.ParentContact); }
            set { Attributes[Metadata.ParentContact] = value; }
        }

        public string Email
        {
            get { return GetAttributeValue<string>(Metadata.Email); }
            set { Attributes[Metadata.Email] = value; }
        }
    }
}
