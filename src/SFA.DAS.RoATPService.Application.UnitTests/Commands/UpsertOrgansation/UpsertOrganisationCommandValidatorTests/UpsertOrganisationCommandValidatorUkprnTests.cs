using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Application.Common.Validators;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation.UpsertOrganisationCommandValidatorTests;

[TestFixture]
public class UpsertOrganisationCommandValidatorUkprnTests
{
    [InlineAutoData(true, 1234567)]
    [InlineAutoData(true, 123456789)]
    [InlineAutoData(true, 23456789)]
    [InlineAutoData(false, 1234567)]
    [InlineAutoData(false, 123456789)]
    [InlineAutoData(false, 23456789)]
    public async Task ValidateUkprn_ShouldBeCorrectFormat(bool isNewProvider, int ukprn, UpsertOrganisationCommand command)
    {
        command.IsNewOrganisation = isNewProvider;
        command.Ukprn = ukprn;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.UkprnFormatValidationMessage);
    }

    [Test, AutoData]
    public async Task ValidateUkprn_IsNewOrganisation_UkprnNotOnRegister_Valid(UpsertOrganisationCommand command)
    {
        command.IsNewOrganisation = true;
        command.Ukprn = UpsertOrganisationCommandValidatorTestHelper.AbsentUkprn;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }

    [Test, AutoData]
    public async Task ValidateUkprn_IsNewOrganisation_UkprnOnRegister_Invalid(UpsertOrganisationCommand command)
    {
        command.IsNewOrganisation = true;
        command.Ukprn = UpsertOrganisationCommandValidatorTestHelper.ExistingUkprn;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UpsertOrganisationCommandValidator.UkprnAlreadyOnRegistertErrorMessage);
    }

    [Test, AutoData]
    public async Task ValidateUkprn_IsExistingProvider_UkprnOnRegister_Valid(UpsertOrganisationCommand command)
    {
        command.IsNewOrganisation = false;
        command.Ukprn = UpsertOrganisationCommandValidatorTestHelper.ExistingUkprn;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }

    [Test, AutoData]
    public async Task ValidateUkprn_IsExistingProvider_UkprnNotOnRegister_Invalid(UpsertOrganisationCommand command)
    {
        command.IsNewOrganisation = false;
        command.Ukprn = UpsertOrganisationCommandValidatorTestHelper.AbsentUkprn;
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UpsertOrganisationCommandValidator.UkprnNotOnRegisterErrorMessage);
    }
}
