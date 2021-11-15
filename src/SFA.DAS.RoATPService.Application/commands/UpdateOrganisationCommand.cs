using System;

namespace SFA.DAS.RoATPService.Application.Commands
{
    public class UpdateOrganisationCommand
    {
        public int ProviderTypeId { get; set; }
        public int OrganisationTypeId { get; set; }
        public string LegalName { get; set; }
        public string TradingName { get; set; }
        public string CharityNumber { get; set; }
        public string CompanyNumber { get; set; }
        public string Username { get; set; }
        public DateTime ApplicationDeterminedDate { get; set; }
        public Guid OrganisationId { get; set; }
    }
}