using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpsertOrgansation.UpsertOrganisationCommandValidatorTests;

[TestFixture]
public class UpsertOrganisationCommandValidatorTradingNameTests
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
    public async Task Validate_NoTradingName_IsValid()
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.TradingName);
    }

    [TestCase(199, true)]
    [TestCase(200, true)]
    [TestCase(201, false)]
    public async Task Validate_TradingName_Length_Validation(int lengthOfName, bool isValid)
    {
        var sut = UpsertOrganisationCommandValidatorTestHelper.GetValidator();

        _command.TradingName = new string('A', lengthOfName);
        var result = await sut.TestValidateAsync(_command);

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.TradingName);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.TradingName)
                .WithErrorMessage(UpsertOrganisationCommandValidator.TradingNameTooLongErrorMessage);
        }
    }
}
