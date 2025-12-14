using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation.UpsertOrganisationCommandValidatorTests;

[TestFixture]
public class UpsertOrganisationCommandValidatorCompanyNumberTests
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
    public async Task Validate_EmptyCompanyNumber_IsValid(string companyNumber)
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = companyNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CompanyNumber);
    }

    [Test]
    public async Task Validate_CompanyNumberNotAlreadyUsed_IsValid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = UpsertOrganisationCommandValidatorTestHelper.UnmatchedCompanyNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CompanyNumber);
    }

    [TestCase("1")]
    [TestCase("12")]
    [TestCase("123")]
    [TestCase("1234")]
    [TestCase("12345")]
    [TestCase("123456")]
    [TestCase("1234567")]
    [TestCase("!1234567")]
    public async Task Validate_IncorrectFormat_Invalid(string companyNumber)
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = companyNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CompanyNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CompanyNumberIsInvalidErrorMessage);
    }

    [Test]
    public async Task Validate_CompanyNumberAlreadyUsed_IsInvalid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = UpsertOrganisationCommandValidatorTestHelper.ExistingCompanyNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CompanyNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CompanyNumberIsUsedErrorMessage);
    }
}
