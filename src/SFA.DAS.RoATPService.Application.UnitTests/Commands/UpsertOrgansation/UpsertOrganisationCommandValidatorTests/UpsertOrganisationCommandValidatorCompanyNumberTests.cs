using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
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
    public async Task ValidateCompanyNumber_IsNewOrganisation_Empty_IsValid(string companyNumber)
    {
        _command.CompanyNumber = companyNumber;
        _command.IsNewOrganisation = true;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldNotHaveValidationErrorFor(c => c.CompanyNumber);
    }

    [Test]
    public async Task ValidateCompanyNumber_IsNewOrganisation_NotAlreadyUsed_IsValid()
    {
        _command.CompanyNumber = UpsertOrganisationCommandValidatorTestHelper.UnmatchedCompanyNumber;
        _command.IsNewOrganisation = true;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldNotHaveValidationErrorFor(c => c.CompanyNumber);
    }

    [TestCase("1", true)]
    [TestCase("!1234567", true)]
    [TestCase("1", false)]
    [TestCase("!1234567", false)]
    public async Task ValidateCompanyNumber_IncorrectFormat_IsInvalid(string companyNumber, bool isNewProvider)
    {
        _command.CompanyNumber = companyNumber;
        _command.IsNewOrganisation = isNewProvider;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldHaveValidationErrorFor(c => c.CompanyNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CompanyNumberIsInvalidErrorMessage);
    }

    [Test]
    public async Task ValidateCompanyNumber_IsNewOrganisation_AlreadyUsed_IsInvalid()
    {
        _command.CompanyNumber = UpsertOrganisationCommandValidatorTestHelper.ExistingCompanyNumber;
        _command.IsNewOrganisation = true;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldHaveValidationErrorFor(c => c.CompanyNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CompanyNumberIsUsedErrorMessage);
    }

    [Test]
    public async Task ValidateCompanyNumber_IsNotNewProvider_AlreadyUsed_IsValid()
    {
        _command.CompanyNumber = UpsertOrganisationCommandValidatorTestHelper.ExistingCompanyNumber;
        _command.IsNewOrganisation = false;
        var repoMock = UpsertOrganisationCommandValidatorTestHelper.GetOrganisationRepositoryMock();
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldNotHaveValidationErrorFor(c => c.CompanyNumber);
        repoMock.Verify(r => r.GetOrganisationByUkprn(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
