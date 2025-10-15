using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
public class GetRemovedReasonsQueryResult
{
    public IEnumerable<RemovedReasonSummary> ReasonsForRemoval { get; set; } = Enumerable.Empty<RemovedReasonSummary>();
}