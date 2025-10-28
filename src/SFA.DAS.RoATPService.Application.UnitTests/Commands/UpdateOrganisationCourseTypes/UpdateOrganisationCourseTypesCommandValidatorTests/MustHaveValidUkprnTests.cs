using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

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

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.UkprnIsRequiredMessage);
    }

    [Test]
    [MoqInlineAutoData(1234567)]
    [MoqInlineAutoData(123456789)]
    public async Task Ukprn_IsInvalidFormat_FailsValidation(
        int ukprn,
        string userId,
        UpdateOrganisationCourseTypesValidator sut)
    {
        UpdateOrganisationCourseTypesCommand command = new(ukprn, [], userId);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.UkprnFormatValidationMessage);
    }


    [Test, RecursiveMoqAutoData]
    public async Task Ukprn_Found_PassesValidation(
    [Frozen] Mock<ICourseTypesRepository> courseTypesRepositoryMock,
    [Greedy] UpdateOrganisationCourseTypesValidator sut,
    Organisation organisation)
    {
        UpdateOrganisationCourseTypesCommand command = new(10012002, [], "requestingUserId");
        courseTypesRepositoryMock.Setup(c => c.GetAllCourseTypes(It.IsAny<CancellationToken>())).ReturnsAsync(() => []);
        sut = new(courseTypesRepositoryMock.Object);

        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }
}
