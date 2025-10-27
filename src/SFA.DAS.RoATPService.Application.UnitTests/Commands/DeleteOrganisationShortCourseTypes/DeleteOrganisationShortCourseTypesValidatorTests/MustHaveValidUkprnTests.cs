using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.DeleteOrganisationShortCourseTypes.DeleteOrganisationShortCourseTypesValidatorTests;
public class MustHaveValidUkprnTests
{
    [Test]
    [MoqInlineAutoData(0)]
    [MoqInlineAutoData(-1)]
    public async Task Ukprn_IsEmpty_FailsValidation(
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

    [Test]
    [MoqInlineAutoData(20000000)]
    [MoqInlineAutoData(1000000)]
    public async Task Ukprn_IsInvalidFormat_FailsValidation(
        int ukprn,
        string requestingUserId,
        DeleteOrganisationShortCourseTypesValidator sut)
    {
        // Arrange
        DeleteOrganisationShortCourseTypesCommand command = new(ukprn, requestingUserId);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.UkprnFormatValidationMessage);
    }
}