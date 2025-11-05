using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetRemovedReasons;
public class GetRemovedReasonsQueryResultTests
{
    [Test, RecursiveMoqAutoData]

    public void GetRemovedReasonsQueryResult_MappsDataCorrectly(
        RemovedReason removedReason)
    {
        // Arrange
        RemovedReasonSummary removedReasonSummary = removedReason;
        GetRemovedReasonsQueryResult sut = new() { ReasonsForRemoval = [removedReasonSummary] };

        // Act
        var result = sut.ReasonsForRemoval.First();

        // Assert
        result.Id.Should().Be(removedReason.Id);
        result.Description.Should().Be(removedReason.Description);
    }
}