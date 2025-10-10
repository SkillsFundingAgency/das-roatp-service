using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.CourseTypesControllerTests;
public class CourseTypesControllerGetAllCourseTypesTests
{
    [Test, MoqAutoData]

    public async Task UpdateCourseTypes_CallsMediatorAndReturnsOkResponse(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] CourseTypesController sut,
        GetAllCourseTypesResult expectedResult
        )
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(It.IsAny<GetAllCourseTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

        // Act
        var result = await sut.GetAllCourseTypes(CancellationToken.None);

        // Assert
        result.As<OkObjectResult>().Value.Should().Be(expectedResult);
        mediatorMock.Verify(m => m.Send(It.IsAny<GetAllCourseTypesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}