using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation.UpsertOrganisationCommandValidatorTests;

[TestFixture]
public class UpsertOrganisationCommandValidatorOrganisationTypeIdTests
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

    [Test]
    public async Task Validate_OrganisationTypeId_IsValid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationTypeId);
    }

    [Test]
    public async Task Validate_OrganisationTypeId_Null_IsInvalid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.OrganisationTypeId = null;

        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationTypeId).WithErrorMessage(UpsertOrganisationCommandValidator.OrganisationTypeIdIsRequiredErrorMessage);
    }

    [Test]
    public async Task Validate_OrganisationTypeId_Invalid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        _command.OrganisationTypeId = UpsertOrganisationCommandValidatorTestHelper.InvalidOrganisationTypeId;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationTypeId).WithErrorMessage(UpsertOrganisationCommandValidator.OrganisationTypeIdShouldBeValidErrorMessage);
    }
}
