using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Api.UnitTests.TestHelpers;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationsControllerTests;
public class OrganisationsControllerPostOrganisationTests
{
    [Test, MoqAutoData]
    public async Task PostOrganisation_ValidResponse_ReturnsCreated(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        int ukprn,
        PostOrganisationCommand model,
        string userId,
        string getOrganisationByUkprnLink,
        CancellationToken cancellationToken)
    {
        sut.AddUrlHelperMock()
            .AddUrlForRoute("GetOrganisationByUkprn", getOrganisationByUkprnLink);

        ValidatedResponse<SuccessModel> validatedResponse = new(new SuccessModel(true));

        mediatorMock
            .Setup(m => m.Send(It.IsAny<PostOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        var result = await sut.PostOrganisation(model, cancellationToken) as CreatedResult;

        result.Should().NotBeNull();
        result!.Location.Should().Be(getOrganisationByUkprnLink);
    }

    [Test, MoqAutoData]
    public async Task PostOrganisation_InvalidResponse_ReturnsBadRequestWithErrors(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationsController sut,
        int ukprn,
        PostOrganisationCommand model,
        string userId,
        List<ValidationError> errors,
        CancellationToken cancellationToken)
    {
        var validatedResponse = new ValidatedResponse<SuccessModel>(errors);
        mediatorMock
            .Setup(m => m.Send(It.IsAny<PostOrganisationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validatedResponse);

        var result = await sut.PostOrganisation(model, cancellationToken);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.EqualTo(errors));
    }
}
