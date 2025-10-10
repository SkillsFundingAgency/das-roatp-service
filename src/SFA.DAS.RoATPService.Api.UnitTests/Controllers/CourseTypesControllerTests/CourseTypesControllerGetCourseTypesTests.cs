using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetCourseTypes;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.CourseTypesControllerTests;
public class CourseTypesControllerGetCourseTypesTests
{
    [Test, MoqAutoData]

    public async Task UpdateCourseTypes_CallsMediatorAndReturnsCourseTypes_OkResponse(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseTypesController sut,
        GetCourseTypesResult expectedResult
        )
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

        // Act
        var result = await sut.GetCourseTypes(CancellationToken.None);

        // Assert
        result.As<OkObjectResult>().Value.Should().Be(expectedResult);
        mediatorMock.Verify(m => m.Send(It.IsAny<GetCourseTypesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]

    public async Task UpdateCourseTypes_CallsMediatorAndReturnsNull_NotFoundResponse(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseTypesController sut
        )
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        // Act
        var result = await sut.GetCourseTypes(CancellationToken.None);

        // Assert
        result.As<NotFoundResult>().Should().NotBeNull();
    }
}