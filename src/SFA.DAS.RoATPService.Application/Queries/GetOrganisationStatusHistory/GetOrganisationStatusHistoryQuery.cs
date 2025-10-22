using MediatR;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;
public record GetOrganisationStatusHistoryQuery(int Ukprn) : IRequest<GetOrganisationStatusHistoryQueryResult>;
