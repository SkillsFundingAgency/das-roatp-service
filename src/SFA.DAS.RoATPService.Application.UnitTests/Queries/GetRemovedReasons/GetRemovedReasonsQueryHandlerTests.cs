using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetRemovedReasons;
public class GetRemovedReasonsQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]

    public async Task Handle_GetAllRemovedReasons_ReturnsRemovedReasons(
        [Frozen] Mock<IRemovedReasonsRepository> removedReasonsRepositoryMock,
        GetRemovedReasonsQueryHandler sut,
        GetRemovedReasonsQuery query,
        List<RemovedReason> expectedResponse)
    {
        // Arrange
        removedReasonsRepositoryMock.Setup(m => m.GetAllRemovedReasons(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.ReasonsForRemoval.Should().BeEquivalentTo(expectedResponse);
        removedReasonsRepositoryMock.Verify(m => m.GetAllRemovedReasons(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]

    public async Task Handle_GetAllRemovedReasons_ReturnsEmpty(
        [Frozen] Mock<IRemovedReasonsRepository> removedReasonsRepositoryMock,
        GetRemovedReasonsQueryHandler sut,
        GetRemovedReasonsQuery query)
    {
        // Arrange
        List<RemovedReason> expectedResponse = new();
        removedReasonsRepositoryMock.Setup(m => m.GetAllRemovedReasons(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.ReasonsForRemoval.Should().BeEmpty();
    }
}