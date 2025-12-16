using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation.UpsertOrganisationCommandValidatorTests;

[TestFixture]
public class UpsertOrganisationCommandValidatorRequestingUserIdTests
{
    private UpsertOrganisationCommand _command;
    [SetUp]
    public void SetUp()
    {
        _command = new UpsertOrganisationCommand
        {
            Ukprn = UpsertOrganisationCommandValidatorTestHelper.AbsentUkprn,
            ProviderType = ProviderType.Main,
            OrganisationTypeId = 1,
            LegalName = "provider legal name",
            RequestingUserId = UpsertOrganisationCommandValidatorTestHelper.ValidUserId
        };
    }

    [Test]
    public async Task Validate_RequestingUserId_IsPresent()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.RequestingUserId);
    }

    [TestCase("")]
    [TestCase(null)]
    public async Task Validate_RequestingUserId_NotSet(string userId)
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        _command.RequestingUserId = userId;
        var result = await sut.TestValidateAsync(_command);

        result.ShouldHaveValidationErrorFor(c => c.RequestingUserId)
                .WithErrorMessage(UpsertOrganisationCommandValidator.RequestingUserIdRequiredErrorMessage);

    }
}
