using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationAllowedShortCourses.UpdateOrganisationAllowedShortCoursesCommandValidatorTests;
public class MustHaveRequestingUserId
{
    [Test]
    [RecursiveMoqInlineAutoData("")]
    [RecursiveMoqInlineAutoData(null)]
    public async Task RequestingUserId_IsEmpty_FailsValidation(
        string userId,
        UpdateOrganisationAllowedShortCoursesCommandValidator sut,
        int ukprn)
    {
        UpdateOrganisationAllowedShortCoursesCommand command = new(ukprn, [], userId);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.RequestingUserId).WithErrorMessage(UpdateOrganisationAllowedShortCoursesCommandValidator.RequestingUserIdIsRequiredMessage);
    }
}
