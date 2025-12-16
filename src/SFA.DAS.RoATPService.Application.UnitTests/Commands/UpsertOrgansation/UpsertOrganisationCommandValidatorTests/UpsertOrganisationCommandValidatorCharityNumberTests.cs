using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
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
    [TestCase("123456")]
    [TestCase("123456-8")]
    [TestCase("12345678901234")]
    public async Task ValidateCharityNumber_IsValid(string charityNumber)
    {
        _command.CharityNumber = charityNumber;
        _command.IsNewOrganisation = true;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldNotHaveValidationErrorFor(c => c.CharityNumber);
    }

    [Test]
    public async Task ValidateCharityNumber_IsNewOrganisation_DistinctNumber_IsValid()
    {
        _command.CharityNumber = UpsertOrganisationCommandValidatorTestHelper.UnmatchedCharityNumber;
        _command.IsNewOrganisation = true;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

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
    public async Task ValidateCharityNumber_IsNewOrganisation_IncorrectFormat_IsInvalid(string charityNumber)
    {
        _command.CharityNumber = charityNumber;
        _command.IsNewOrganisation = true;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldHaveValidationErrorFor(c => c.CharityNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CharityNumberIsInvalidErrorMessage);
    }

    [Test]
    public async Task ValidateCharityNumber_IsNewOrganisation_AlreadyUsed_IsInvalid()
    {
        _command.CharityNumber = UpsertOrganisationCommandValidatorTestHelper.ExistingCharityNumber;
        _command.IsNewOrganisation = true;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldHaveValidationErrorFor(c => c.CharityNumber).WithErrorMessage(UpsertOrganisationCommandValidator.CharityNumberIsUsedErrorMessage);
    }

    [Test]
    public async Task ValidateCharityNumber_IsNotNewProvider_AlreadyUsed_IsValid()
    {
        _command.CharityNumber = UpsertOrganisationCommandValidatorTestHelper.ExistingCharityNumber;
        _command.IsNewOrganisation = false;
        var mockRepo = UpsertOrganisationCommandValidatorTestHelper.GetOrganisationRepositoryMock();
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);

        result.ShouldNotHaveValidationErrorFor(c => c.CharityNumber);
        mockRepo.Verify(r => r.GetOrganisationByUkprn(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
