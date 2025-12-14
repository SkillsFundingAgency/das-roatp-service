using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation.UpsertOrganisationCommandValidatorTests;

[TestFixture]
public class UpsertOrganisationCommandValidatorCharityNumberTests
{
    private UpsertOrganisationCommand _command;
    [SetUp]
    public void SetUp()
    {
        _command = new UpsertOrganisationCommand
        {
            Ukprn = UpsertOrganisationCommandValidatorTestHelper.AbsentUkprn,
            ProviderType = ProviderType.Main,
            OrganisationTypeId = UpsertOrganisationCommandValidatorTestHelper.ValidOrganisationTypeId,
            LegalName = "provider legal name",
            RequestingUserId = UpsertOrganisationCommandValidatorTestHelper.ValidUserId
        };
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public async Task Validate_EmptyCharityNumber_IsValid(string charityNumber)
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = charityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CharityNumber);
    }

    [Test]
    public async Task Validate_CharityNumberNotAlreadyUsed_IsValid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = UpsertOrganisationCommandValidatorTestHelper.UnmatchedCharityNumber;
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
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = charityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CharityNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CharityNumberIsInvalidErrorMessage);
    }

    [TestCase("123456")]
    [TestCase("123456-8")]
    [TestCase("12345678901234")]
    public async Task Validate_CorrectFormat_IsValid(string charityNumber)
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = charityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CharityNumber);
    }

    [Test]
    public async Task Validate_AlreadyUsed_IsInvalid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CharityNumber = UpsertOrganisationCommandValidatorTestHelper.ExistingCharityNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CharityNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CharityNumberIsUsedErrorMessage);
    }
}
