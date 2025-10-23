using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.DeleteOrganisationShortCourseTypes.DeleteOrganisationShortCourseTypesValidatorTests;
public class MustHaveValidUkprnTests
{
    [Test]
    [MoqInlineAutoData(0)]
    [MoqInlineAutoData(-1)]
    public async Task Ukprn_IsEmnpty_FailsValidation(
        int ukprn,
        string requestingUserId,
        DeleteOrganisationShortCourseTypesValidator sut)
    {
        // Arrange
        DeleteOrganisationShortCourseTypesCommand command = new(ukprn, requestingUserId);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(DeleteOrganisationShortCourseTypesValidator.UkprnIsRequiredMessage);
    }

    [Test, MoqAutoData]
    public async Task Ukprn_NotFound_FailsValidation(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        [Greedy] DeleteOrganisationShortCourseTypesValidator sut,
        DeleteOrganisationShortCourseTypesCommand command)
    {
        // Arrange
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(() => null);
        sut = new(organisationsRepositoryMock.Object);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(DeleteOrganisationShortCourseTypesValidator.InvalidUkprnMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Ukprn_Found_PassesValidation(
    [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
    [Greedy] DeleteOrganisationShortCourseTypesValidator sut,
    DeleteOrganisationShortCourseTypesCommand command,
    Organisation organisation)
    {
        // Arrange
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(organisation);
        sut = new(organisationsRepositoryMock.Object);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }
}