using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationCourseTypesControllerTests;

public class UpdateCourseTypesTests
{
    private Mock<IMediator> _mediatorMock;
    private OrganisationCourseTypesController _controller;

    [SetUp]
    public void SetUp()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new OrganisationCourseTypesController(_mediatorMock.Object);
    }

    [Test]
    public async Task UpdateCourseTypes_ValidResponse_ReturnsOk()
    {
        // Arrange
        var ukprn = 12345678;
        UpdateCourseTypesModel model = new([1, 2], "user-1");

        ValidatedResponse validatedResponse = new();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateOrganisationAllowedShortCoursesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await _controller.UpdateCourseTypes(ukprn, model, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    public async Task UpdateCourseTypes_InvalidResponse_ReturnsBadRequestWithErrors()
    {
        // Arrange
        var ukprn = 12345678;
        UpdateCourseTypesModel model = new([1, 2], "user-1");

        List<ValidationError> errors = [new ValidationError("Property1", "Error message")];
        var validatedResponse = new ValidatedResponse(errors);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateOrganisationAllowedShortCoursesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await _controller.UpdateCourseTypes(ukprn, model, CancellationToken.None);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errors));
    }
}
