using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PatchOrganisation.PatchOrganisationCommandValidatorTests;

public class PatchOrganisationCommandValidatorUserIdTests
{
    [Test]
    public async Task Validate_UserId_IsRequired()
    {
        //Arrange
        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, string.Empty, new());
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldHaveValidationErrorFor(c => c.UserId).WithErrorMessage(PatchOrganisationCommandValidator.RequestingUserIdIsRequiredErrorMessage);
    }

    [Test]
    public async Task Validate_UserId_IsValid()
    {
        //Arrange
        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, new());
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldNotHaveValidationErrorFor(c => c.UserId);
    }
}
