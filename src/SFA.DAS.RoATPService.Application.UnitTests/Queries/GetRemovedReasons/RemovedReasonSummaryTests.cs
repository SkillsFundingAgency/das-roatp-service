using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetRemovedReasons;
public class RemovedReasonSummaryTests
{
    [Test, RecursiveMoqAutoData]

    public void RemovedReasonSummary_MapsDataCorrectly(
        RemovedReason removedReason)
    {
        // Act
        RemovedReasonSummary result = removedReason;

        // Assert
        result.Id.Should().Be(removedReason.Id);
        result.Description.Should().Be(removedReason.Description);
    }
}