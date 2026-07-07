namespace SFA.DAS.RoATPService.Application.Events;

public class ProviderStatusChangedEvent
{
    public long Ukprn { get; set; }
    public string Status { get; set; } = string.Empty;
}
