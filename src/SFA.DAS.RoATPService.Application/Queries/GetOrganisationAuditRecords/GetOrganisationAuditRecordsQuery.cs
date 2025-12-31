using MediatR;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationAuditRecords;

public record GetOrganisationAuditRecordsQuery : IRequest<GetOrganisationAuditRecordsQueryResult>;
