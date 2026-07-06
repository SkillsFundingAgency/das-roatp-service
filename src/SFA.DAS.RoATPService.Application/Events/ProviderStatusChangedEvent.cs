namespace SFA.DAS.RoATPService.Application.Events;

public record ProviderStatusChangedEvent(long ukprn, string status);
