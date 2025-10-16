using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
public record RemovedReasonSummary(int Id, string Description)
{
    public static implicit operator RemovedReasonSummary(RemovedReason source) => new(source.Id, source.Reason);
}