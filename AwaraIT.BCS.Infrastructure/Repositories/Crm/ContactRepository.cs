using AwaraIT.BCS.Domain.Extensions;
using AwaraIT.BCS.Domain.Models.Crm;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AwaraIT.BCS.Infrastructure.Repositories.Crm
{
    public class ContactRepository : CrmRepository<Contact>
    {
        public ContactRepository(IOrganizationService service) : base(service) { }

        protected override string EntityName => Contact.EntityLogicalName;

        public Contact GetByFullName(string fullName)
        {
            var res = Get(new[] { new ConditionExpression(Contact.Metadata.FullName, ConditionOperator.Equal, fullName) }, 1,
                Contact.Metadata.FullName, Contact.Metadata.LastName);
            return res.FirstOrDefault();
        }


        public List<Contact> GetByPhoneOrEmail(string phoneNumber = null, string email = null)
        {

            var qx = new QueryExpression(Contact.EntityLogicalName)
            {
                NoLock = true,
                ColumnSet = new ColumnSet(Contact.Metadata.Id),
            };
            qx.Criteria.AddCondition(EntityCommon.StateCode, ConditionOperator.Equal, (int)Contact.Metadata.StateCode.Active);

            var contactDataFilter = qx.Criteria.AddFilter(LogicalOperator.Or);

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var phoneFilter = contactDataFilter.AddFilter(LogicalOperator.Or);
                var phone = phoneNumber.OnlyDigits();
                var conditionPhoneNumber = new[] { phone, $"+{phone}" };
                phoneFilter.AddCondition(Contact.Metadata.MobilePhone, ConditionOperator.In, conditionPhoneNumber);
                phoneFilter.AddCondition(Contact.Metadata.Telephone1, ConditionOperator.In, conditionPhoneNumber);
                phoneFilter.AddCondition(Contact.Metadata.Telephone2, ConditionOperator.In, conditionPhoneNumber);
                phoneFilter.AddCondition(Contact.Metadata.Telephone3, ConditionOperator.In, conditionPhoneNumber);
            }

            if (!string.IsNullOrEmpty(email))
            {
                var emailFilter = contactDataFilter.AddFilter(LogicalOperator.Or);
                emailFilter.AddCondition(Contact.Metadata.Email, ConditionOperator.Equal, email);
                emailFilter.AddCondition(Contact.Metadata.Email2, ConditionOperator.Equal, email);
                emailFilter.AddCondition(Contact.Metadata.Email3, ConditionOperator.Equal, email);
            }

            var res = Get(qx);
            return res;
        }

        public Contact GetById(Guid id)
        {
            var res = Get(id, true);
            return res;
        }
    }
}
