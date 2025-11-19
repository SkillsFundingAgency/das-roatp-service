using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;

[TestFixture]
public class PostOrganisationCommandValidatorCompanyNumberTests
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
    public async Task Validate_EmptyCompanyNumber_IsValid(string companyNumber)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = companyNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.CompanyNumber);
    }

    [Test]
    public async Task Validate_CompanyNumberNotAlreadyUsed_IsValid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = PostOrganisationCommandValidatorTestHelper.UnmatchedCompanyNumber;
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
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = companyNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CompanyNumber).WithErrorMessage(PostOrganisationCommandValidator.CompanyNumberIsInvalidErrorMessage);
    }

    [Test]
    public async Task Validate_CompanyNumberAlreadyUsed_IsInvalid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.CompanyNumber = PostOrganisationCommandValidatorTestHelper.ExistingCompanyNumber;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.CompanyNumber).WithErrorMessage(PostOrganisationCommandValidator.CompanyNumberIsUsedErrorMessage);
    }
}
