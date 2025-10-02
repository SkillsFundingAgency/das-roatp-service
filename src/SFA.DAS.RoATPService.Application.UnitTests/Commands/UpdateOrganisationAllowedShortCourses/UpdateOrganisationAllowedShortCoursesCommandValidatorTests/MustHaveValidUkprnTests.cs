using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationAllowedShortCourses.UpdateOrganisationAllowedShortCoursesCommandValidatorTests;

public class MustHaveValidUkprnTests
{
    [Test]
    [MoqInlineAutoData(0)]
    [MoqInlineAutoData(-1)]
    public async Task Ukprn_IsEmpty_FailsValidation(
        int ukprn,
        UpdateOrganisationAllowedShortCoursesCommandValidator sut)
    {
        UpdateOrganisationAllowedShortCoursesCommand command = new(ukprn, []);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UpdateOrganisationAllowedShortCoursesCommandValidator.UkprnIsRequiredMessage);
    }

    [Test, MoqAutoData]
    public async Task Ukprn_NotFound_FailsValidation(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        [Frozen] Mock<ICourseTypesRepository> courseTypesRepositoryMock,
        [Greedy] UpdateOrganisationAllowedShortCoursesCommandValidator sut,
        UpdateOrganisationAllowedShortCoursesCommand command)
    {
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(() => null);
        courseTypesRepositoryMock.Setup(c => c.GetAllCourseTypes(It.IsAny<CancellationToken>())).ReturnsAsync(() => []);
        sut = new(organisationsRepositoryMock.Object, courseTypesRepositoryMock.Object);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UpdateOrganisationAllowedShortCoursesCommandValidator.InvalidUkprnMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Ukprn_Found_PassesValidation(
    [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
    [Frozen] Mock<ICourseTypesRepository> courseTypesRepositoryMock,
    [Greedy] UpdateOrganisationAllowedShortCoursesCommandValidator sut,
    UpdateOrganisationAllowedShortCoursesCommand command,
    Organisation organisation)
    {
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(organisation);
        courseTypesRepositoryMock.Setup(c => c.GetAllCourseTypes(It.IsAny<CancellationToken>())).ReturnsAsync(() => []);
        sut = new(organisationsRepositoryMock.Object, courseTypesRepositoryMock.Object);

        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }
}
