using MediatR;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
public class GetRemovedReasonsQueryHandler(IRemovedReasonsRepository _removedReasonsRepository) : IRequestHandler<GetRemovedReasonsQuery, GetRemovedReasonsQueryResult>
{
    public async Task<GetRemovedReasonsQueryResult> Handle(GetRemovedReasonsQuery request, CancellationToken cancellationToken)
    {
        List<RemovedReason> Removedreasons = await _removedReasonsRepository.GetAllRemovedReasons(cancellationToken);
    }
}

public class GetRemovedReasonsQuery : IRequest<GetRemovedReasonsQueryResult>
{
}

public class GetRemovedReasonsQueryResult
{
    IEnumerable<RemovedReasonSummary> RemovedReasons { get; set; } = Enumerable.Empty<RemovedReasonSummary>();
}