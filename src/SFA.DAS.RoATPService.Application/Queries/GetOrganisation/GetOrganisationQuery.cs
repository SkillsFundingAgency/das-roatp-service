using MediatR;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisation;

public record GetOrganisationQuery(int Ukprn) : IRequest<GetOrganisationQueryResult>;
