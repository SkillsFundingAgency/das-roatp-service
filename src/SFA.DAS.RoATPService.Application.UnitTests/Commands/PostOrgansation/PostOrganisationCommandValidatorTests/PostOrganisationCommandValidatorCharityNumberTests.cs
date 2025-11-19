using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;

[TestFixture]
public class PostOrganisationCommandValidatorCharityNumberTests
{
    private PostOrganisationCommand _command;
    [SetUp]
    public void SetUp()
    {
        _command = new PostOrganisationCommand
        {
            Ukprn = PostOrganisationCommandValidatorTestHelper.AbsentUkprn,
            ProviderType = ProviderType.Main,
            OrganisationTypeId = PostOrganisationCommandValidatorTestHelper.ValidOrganisationTypeId,
            LegalName = "provider legal name",
            RequestingUserId = PostOrganisationCommandValidatorTestHelper.ValidUserId
        };
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task Validate_EmptyCharityNumber_IsValid(string charityNumber)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = charityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CharityNumber);
    }

    [Test]
    public async Task Validate_CharityNumberNotAlreadyUsed_IsValid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = PostOrganisationCommandValidatorTestHelper.UnmatchedCharityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CharityNumber);
    }

    [TestCase("1!")]
    [TestCase("12+")]
    [TestCase("1^23")]
    [TestCase("1234")]
    [TestCase("12345")]
    [TestCase("12345!")]
    [TestCase("123456789012345")]
    public async Task Validate_IncorrectFormat_Invalid(string charityNumber)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = charityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CharityNumber).WithErrorMessage(PostOrganisationCommandValidator.CharityNumberIsInvalidErrorMessage);
    }

    [TestCase("123456")]
    [TestCase("123456-8")]
    [TestCase("12345678901234")]
    public async Task Validate_CorrectFormat_IsValid(string charityNumber)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = charityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CharityNumber);
    }

    [Test]
    public async Task Validate_AlreadyUsed_IsInvalid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = PostOrganisationCommandValidatorTestHelper.ExistingCharityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CharityNumber).WithErrorMessage(PostOrganisationCommandValidator.CharityNumberIsUsedErrorMessage);
    }
}
