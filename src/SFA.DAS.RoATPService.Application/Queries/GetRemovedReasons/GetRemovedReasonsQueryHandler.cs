using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
public class GetRemovedReasonsQueryHandler(IRemovedReasonsRepository _removedReasonsRepository, ILogger<GetRemovedReasonsQueryHandler> _logger) : IRequestHandler<GetRemovedReasonsQuery, GetRemovedReasonsQueryResult>
{
    public async Task<GetRemovedReasonsQueryResult> Handle(GetRemovedReasonsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handle request for get all removed reasons");

        List<RemovedReason> removedReasons = await _removedReasonsRepository.GetAllRemovedReasons(cancellationToken);

        return new() { ReasonsForRemoval = removedReasons.Select(r => (RemovedReasonSummary)r) };
    }
}