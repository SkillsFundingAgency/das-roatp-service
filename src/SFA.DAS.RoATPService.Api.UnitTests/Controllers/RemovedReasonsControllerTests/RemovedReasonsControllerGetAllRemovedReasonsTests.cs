using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.RemovedReasonsControllerTests;
public class RemovedReasonsControllerGetAllRemovedReasonsTests
{
    [Test, MoqAutoData]
    public async Task GetAllRemovedReasons_CallsMediator_ReturnsOkResponse(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] RemovedReasonsController sut,
        GetRemovedReasonsQueryResult expectedResult
        )
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetRemovedReasonsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

        // Act
        var result = await sut.GetAllRemovedReasons(CancellationToken.None);

        // Assert
        result.As<OkObjectResult>().Value.Should().Be(expectedResult);
        mediatorMock.Verify(m => m.Send(It.IsAny<GetRemovedReasonsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}