using MediatR;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;

public record GetOrganisationTypesQuery(int? ProviderTypeId) : IRequest<ValidatedResponse<GetOrganisationTypesQueryResult>>;
