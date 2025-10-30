using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationsControllerTests;
public class OrganisationsControllerPatchOrganisationTests
{
    [Test, MoqAutoData]
    public async Task PatchOrganisation_ValidResponse_ReturnsNoContent(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        int ukprn,
        JsonPatchDocument<PatchOrganisationModel> model,
        string userId,
        CancellationToken cancellationToken)
    {
        // Arrange
        ValidatedResponse<SuccessModel> validatedResponse = new(new SuccessModel(true));

        mediatorMock
            .Setup(m => m.Send(It.IsAny<PatchOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.PatchOrganisation(ukprn, model, userId, cancellationToken);

        // Assert
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test, MoqAutoData]
    public async Task PatchOrganisation_InvalidResponse_ReturnsBadRequestWithErrors(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        int ukprn,
        JsonPatchDocument<PatchOrganisationModel> model,
        string userId,
        List<ValidationError> errors,
        CancellationToken cancellationToken)
    {
        // Arrange
        var validatedResponse = new ValidatedResponse<SuccessModel>(errors);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<PatchOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.PatchOrganisation(ukprn, model, userId, cancellationToken);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errors));
    }

    [Test, MoqAutoData]
    public async Task PatchOrganisation_UkprnNotFound_ReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        int ukprn,
        JsonPatchDocument<PatchOrganisationModel> model,
        string userId,
        List<ValidationError> errors,
        CancellationToken cancellationToken)
    {
        // Arrange
        var validatedResponse = new ValidatedResponse<SuccessModel>(new SuccessModel(false));
        mediatorMock
            .Setup(m => m.Send(It.IsAny<PatchOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        // Act
        var result = await sut.PatchOrganisation(ukprn, model, userId, cancellationToken);

        // Assert
        var notFoundResult = result as NotFoundResult;
        Assert.That(notFoundResult, Is.Not.Null);
    }
}
