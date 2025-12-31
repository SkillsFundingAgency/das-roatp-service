using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationAuditRecords;

public class GetOrganisationAuditRecordsQueryHandler(IAuditsRepository _auditsRepository) : IRequestHandler<GetOrganisationAuditRecordsQuery, GetOrganisationAuditRecordsQueryResult>
{
    public async Task<GetOrganisationAuditRecordsQueryResult> Handle(GetOrganisationAuditRecordsQuery request, CancellationToken cancellationToken)
    {
        List<OrganisationAudit> records = await _auditsRepository.GetOrganisationAuditRecords(cancellationToken);
        return new GetOrganisationAuditRecordsQueryResult(records.ConvertAll(r => (OrganisationAuditRecord)r));
    }
}
