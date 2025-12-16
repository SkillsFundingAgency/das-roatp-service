using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;

namespace SFA.DAS.RoATPService.Application.Api.Models;

public class UpdateOrganisationModel
{
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public string CompanyNumber { get; set; }
    public string CharityNumber { get; set; }
    public Domain.Common.ProviderType ProviderType { get; set; }
    public int? OrganisationTypeId { get; set; }
    public string RequestingUserId { get; set; }

    public static implicit operator UpsertOrganisationCommand(UpdateOrganisationModel model) =>
        new UpsertOrganisationCommand
        {
            LegalName = model.LegalName,
            TradingName = model.TradingName,
            CompanyNumber = model.CompanyNumber,
            CharityNumber = model.CharityNumber,
            ProviderType = model.ProviderType,
            OrganisationTypeId = model.OrganisationTypeId,
            RequestingUserId = model.RequestingUserId
        };
}
