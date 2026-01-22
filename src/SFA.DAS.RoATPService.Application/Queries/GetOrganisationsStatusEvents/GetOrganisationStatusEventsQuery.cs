using MediatR;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;

public record GetOrganisationStatusEventsQuery(int SinceEventId, int PageSize, int PageNumber) : IRequest<GetOrganisationStatusEventsQueryResult>;
