using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers.OrganisationCourseTypesControllerTests;
public class DeleteShortCourseTypesTests
{
    [Test, MoqAutoData]
    public async Task DeleteShortCourseTypes_ResponseIsValid_ReturnsNoContent(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationCourseTypesController sut,
        DeleteOrganisationShortCourseTypesCommand command)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse());

        // Act
        var result = await sut.DeleteShortCourseTypes(command.Ukprn, command.RequestingUserId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test, MoqAutoData]
    public async Task DeleteShortCourseTypes_ResponseIsInValid_ReturnsBadRequestWithValidatioErrors(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationCourseTypesController sut,
        DeleteOrganisationShortCourseTypesCommand command,
        List<ValidationError> validationErrors)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidatedResponse(validationErrors));

        // Act
        var result = await sut.DeleteShortCourseTypes(command.Ukprn, command.RequestingUserId, CancellationToken.None);
        var response = result.As<BadRequestObjectResult>();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        response.Value.Should().BeEquivalentTo(validationErrors);
    }

    [Test, MoqAutoData]
    public async Task DeleteShortCourseTypes_UkprnNotFound_ReturnsNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationCourseTypesController sut,
        DeleteOrganisationShortCourseTypesCommand command)
    {
        // Arrange
        mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        // Act
        var result = await sut.DeleteShortCourseTypes(command.Ukprn, command.RequestingUserId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }
}