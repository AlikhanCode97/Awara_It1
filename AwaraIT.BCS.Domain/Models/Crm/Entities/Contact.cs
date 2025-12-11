using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace AwaraIT.BCS.Domain.Models.Crm.Entities
{
    [EntityLogicalName(EntityLogicalName)]
    public class Contact : BaseEntity
    {
        public Contact() : base(EntityLogicalName) { }

        public static class Metadata
        {
            public const string FullName = "fullname";
            public const string FirstName = "firstname";
            public const string LastName = "lastname";
            public const string Email = "emailaddress1";
            public const string Email2 = "emailaddress2";
            public const string Email3 = "emailaddress3";
            public const string MobilePhone = "mobilephone";
            public const string Telephone1 = "telephone1";
            public const string Telephone2 = "telephone2";
            public const string Telephone3 = "telephone3";
            public const string Id = "contactid";


            public enum StateCode
            {
                Active = 0,
                Inactive = 1
            }
        }

        public const string EntityLogicalName = "contact";

        public Metadata.StateCode? StateCode
        {
            get { return (Metadata.StateCode?)GetAttributeValue<OptionSetValue>(EntityCommon.StateCode)?.Value; }
            set { Attributes[EntityCommon.StateCode] = value != null ? new OptionSetValue((int)value.Value) : null; }
        }

        public string FullName
        {
            get { return GetAttributeValue<string>(Metadata.FullName); }
            set { Attributes[Metadata.FullName] = value; }
        }

        public string FirstName
        {
            get { return GetAttributeValue<string>(Metadata.FirstName); }
            set { Attributes[Metadata.FirstName] = value; }
        }

        public string LastName
        {
            get { return GetAttributeValue<string>(Metadata.LastName); }
            set { Attributes[Metadata.LastName] = value; }
        }
        public string Email
        {
            get { return GetAttributeValue<string>(Metadata.Email); }
            set { Attributes[Metadata.Email] = value; }
        }
        public string Email2
        {
            get { return GetAttributeValue<string>(Metadata.Email2); }
            set { Attributes[Metadata.Email2] = value; }
        }

        public string Email3
        {
            get { return GetAttributeValue<string>(Metadata.Email3); }
            set { Attributes[Metadata.Email3] = value; }
        }
        public string MobilePhone
        {
            get { return GetAttributeValue<string>(Metadata.MobilePhone); }
            set { Attributes[Metadata.MobilePhone] = value; }
        }
        public string Telephone1
        {
            get { return GetAttributeValue<string>(Metadata.Telephone1); }
            set { Attributes[Metadata.Telephone1] = value; }
        }

        public string Telephone2
        {
            get { return GetAttributeValue<string>(Metadata.Telephone2); }
            set { Attributes[Metadata.Telephone2] = value; }
        }

        public string Telephone3
        {
            get { return GetAttributeValue<string>(Metadata.Telephone3); }
            set { Attributes[Metadata.Telephone3] = value; }
        }

    }
}
