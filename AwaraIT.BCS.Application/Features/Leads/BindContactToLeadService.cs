using AwaraIT.BCS.Application.Core;
using AwaraIT.BCS.Domain.Models.Crm.Entities;
using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace AwaraIT.BCS.Application.Features.Leads
{
    public class BindContactToLeadService
    {

        private readonly ContactRepository _contactRepository;
        private readonly LeadRepository _leadRepository;



        public BindContactToLeadService(IOrganizationService service)
        {
            _contactRepository = new ContactRepository(service);
            _leadRepository = new LeadRepository(service);
        }

        public Result<Core.Void> Execute(Lead lead)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(lead.MobilePhone) && string.IsNullOrWhiteSpace(lead.Email))
                {
                    return Result<Core.Void>.Success();
                }

                var contacts = _contactRepository.GetByPhoneOrEmail(lead.MobilePhone, lead.Email);
                if (contacts != null && contacts.Any())
                {
                    var contactReference = contacts.First().ToEntityReference();
                    _leadRepository.Update(new Lead
                    {
                        Id = lead.Id,
                        ParentContact = contactReference
                    });
                }

                return Result<Core.Void>.Success();
            }
            catch (Exception ex)
            {
                return Result<Core.Void>.Failure($"Exсeption in BindContactToLeadService {ex.Message}");
            }

        }
    }
}
