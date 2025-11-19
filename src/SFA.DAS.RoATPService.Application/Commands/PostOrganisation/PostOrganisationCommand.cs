using MediatR;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

namespace SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
public class PostOrganisationCommand : IRequest<ValidatedResponse<SuccessModel>>
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public string CompanyNumber { get; set; }
    public string CharityNumber { get; set; }
    public Domain.Common.ProviderType ProviderType { get; set; }
    public int? OrganisationTypeId { get; set; }
    public string RequestingUserId { get; set; }
}
