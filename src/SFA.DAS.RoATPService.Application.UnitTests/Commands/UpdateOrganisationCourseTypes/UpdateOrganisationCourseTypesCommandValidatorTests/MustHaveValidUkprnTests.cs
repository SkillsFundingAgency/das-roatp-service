using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationCourseTypes.UpdateOrganisationCourseTypesCommandValidatorTests;

public class MustHaveValidUkprnTests
{
    [Test]
    [MoqInlineAutoData(0)]
    [MoqInlineAutoData(-1)]
    public async Task Ukprn_IsEmpty_FailsValidation(
        int ukprn,
        string userId,
        UpdateOrganisationCourseTypesValidator sut)
    {
        UpdateOrganisationCourseTypesCommand command = new(ukprn, [], userId);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UpdateOrganisationCourseTypesValidator.UkprnIsRequiredMessage);
    }

    [Test, MoqAutoData]
    public async Task Ukprn_NotFound_FailsValidation(
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        [Frozen] Mock<ICourseTypesRepository> courseTypesRepositoryMock,
        [Greedy] UpdateOrganisationCourseTypesValidator sut,
        UpdateOrganisationCourseTypesCommand command)
    {
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(() => null);
        courseTypesRepositoryMock.Setup(c => c.GetAllCourseTypes(It.IsAny<CancellationToken>())).ReturnsAsync(() => []);
        sut = new(organisationsRepositoryMock.Object, courseTypesRepositoryMock.Object);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UpdateOrganisationCourseTypesValidator.InvalidUkprnMessage);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Ukprn_Found_PassesValidation(
    [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
    [Frozen] Mock<ICourseTypesRepository> courseTypesRepositoryMock,
    [Greedy] UpdateOrganisationCourseTypesValidator sut,
    UpdateOrganisationCourseTypesCommand command,
    Organisation organisation)
    {
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(organisation);
        courseTypesRepositoryMock.Setup(c => c.GetAllCourseTypes(It.IsAny<CancellationToken>())).ReturnsAsync(() => []);
        sut = new(organisationsRepositoryMock.Object, courseTypesRepositoryMock.Object);

        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }
}
