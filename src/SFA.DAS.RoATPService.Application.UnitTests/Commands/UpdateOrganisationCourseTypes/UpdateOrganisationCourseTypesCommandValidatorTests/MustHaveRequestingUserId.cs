using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationCourseTypes.UpdateOrganisationCourseTypesCommandValidatorTests;
public class MustHaveRequestingUserId
{
    [Test]
    [RecursiveMoqInlineAutoData("")]
    [RecursiveMoqInlineAutoData(null)]
    public async Task RequestingUserId_IsEmpty_FailsValidation(
        string userId,
        UpdateOrganisationCourseTypesValidator sut,
        int ukprn)
    {
        UpdateOrganisationCourseTypesCommand command = new(ukprn, [], userId);

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.RequestingUserId).WithErrorMessage(UpdateOrganisationCourseTypesValidator.RequestingUserIdIsRequiredMessage);
    }
}
