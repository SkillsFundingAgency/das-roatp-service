using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Application.Common.Validators;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PatchOrganisation.PatchOrganisationCommandValidatorTests;
public class PatchOrganisationCommandValidatorUkprnTests
{


    [Test]
    public async Task Validate_Ukprn_ShouldBeGreaterThanZero()
    {
        //Arrange
        var command = new PatchOrganisationCommand(0, PatchOrganisationCommandValidatorTestHelper.ValidUserId, new());
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.UkprnIsRequiredMessage);
    }

    [TestCase(1234567)]
    [TestCase(123456789)]
    [TestCase(23456789)]
    public async Task Validate_Ukprn_ShouldBeCorrectFormat(int ukprn)
    {
        //Arrange
        var command = new PatchOrganisationCommand(ukprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, new());
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.UkprnFormatValidationMessage);
    }

    [Test]
    public async Task Validate_Ukprn_IsValid()
    {
        //Arrange
        var command = new PatchOrganisationCommand(PatchOrganisationCommandValidatorTestHelper.ValidUkprn, PatchOrganisationCommandValidatorTestHelper.ValidUserId, new());
        var sut = PatchOrganisationCommandValidatorTestHelper.GetValidator();
        //Act
        var result = await sut.TestValidateAsync(command);
        //Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }
}
