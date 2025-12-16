using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationsControllerTests;

public class OrganisationControllerPutOrganisationTests
{
    [Test, MoqAutoData]
    public async Task PutOrganisation_InvokesMediator(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        UpdateOrganisationModel model,
        int ukprn,
        CancellationToken cancellationToken)
    {
        ValidatedResponse<SuccessModel> validatedResponse = new(new SuccessModel(true));
        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpsertOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        await sut.PutOrganisation(ukprn, model, cancellationToken);

        mediatorMock
            .Verify(m => m.Send(
                It.Is<UpsertOrganisationCommand>(c =>
                    !c.IsNewOrganisation &&
                    c.Ukprn == ukprn),
                cancellationToken), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task PutOrganisation_ValidResponse_ReturnsNoContent(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        UpdateOrganisationModel model,
        int ukprn,
        CancellationToken cancellationToken)
    {
        ValidatedResponse<SuccessModel> validatedResponse = new(new SuccessModel(true));

        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpsertOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        var result = await sut.PutOrganisation(ukprn, model, cancellationToken);

        result.As<NoContentResult>().Should().NotBeNull();
    }
    [Test, MoqAutoData]
    public async Task PutOrganisation_InvalidResponse_ReturnsBadRequestWithErrors(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        UpdateOrganisationModel model,
        int ukprn,
        List<ValidationError> errors,
        CancellationToken cancellationToken)
    {
        var validatedResponse = new ValidatedResponse<SuccessModel>(errors);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<UpsertOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        var result = await sut.PutOrganisation(ukprn, model, cancellationToken);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errors));
    }
}
