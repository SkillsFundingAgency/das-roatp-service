using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.DeleteOrganisationShortCourseTypes.DeleteOrganisationShortCourseTypesValidatorTests;
public class MustHaveRequestingUserIdTests
{
    [Test]
    [RecursiveMoqInlineAutoData("")]
    [RecursiveMoqInlineAutoData(null)]
    public async Task RequestingUserId_IsEmptyOrNull_FailsValidation(
        string requestingUserId,
        DeleteOrganisationShortCourseTypesValidator sut,
        int ukprn)
    {
        // Arrange
        DeleteOrganisationShortCourseTypesCommand command = new(ukprn, requestingUserId);

        // Act
        var result = await sut.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.RequestingUserId).WithErrorMessage(DeleteOrganisationShortCourseTypesValidator.RequestingUserIdIsRequiredMessage);
    }
}
