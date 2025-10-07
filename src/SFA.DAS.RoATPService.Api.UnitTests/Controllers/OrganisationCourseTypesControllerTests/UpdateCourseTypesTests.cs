using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationCourseTypesControllerTests;

public class UpdateCourseTypesTests
{
    [Test, MoqAutoData]
    public async Task UpdateCourseTypes_ValidResponse_ReturnsOk(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationCourseTypesController sut,
        int ukprn,
        UpdateCourseTypesModel model)
    {
        // Arrange
        ValidatedResponse validatedResponse = new();

        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateOrganisationCourseTypesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.UpdateCourseTypes(ukprn, model, CancellationToken.None);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test, MoqAutoData]
    public async Task UpdateCourseTypes_InvalidResponse_ReturnsBadRequestWithErrors(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationCourseTypesController sut,
        int ukprn,
        UpdateCourseTypesModel model,
        List<ValidationError> errors)
    {
        // Arrange
        var validatedResponse = new ValidatedResponse(errors);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateOrganisationCourseTypesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.UpdateCourseTypes(ukprn, model, CancellationToken.None);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errors));
    }
}
