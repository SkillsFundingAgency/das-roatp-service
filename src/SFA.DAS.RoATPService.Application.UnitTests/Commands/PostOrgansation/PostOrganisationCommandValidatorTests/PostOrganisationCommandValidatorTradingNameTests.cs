using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;

[TestFixture]
public class PostOrganisationCommandValidatorTradingNameTests
{
    private PostOrganisationCommand _command;
    [SetUp]
    public void SetUp()
    {
        _command = new PostOrganisationCommand
        {
            Ukprn = PostOrganisationCommandValidatorTestHelper.AbsentUkprn,
            ProviderType = ProviderType.Main,
            OrganisationTypeId = 1,
            LegalName = "provider legal name",
            RequestingUserId = PostOrganisationCommandValidatorTestHelper.ValidUserId
        };
    }

    [Test]
    public async Task Validate_NoTradingName_IsValid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.TradingName);
    }

    [TestCase(199, true)]
    [TestCase(200, true)]
    [TestCase(201, false)]
    public async Task Validate_TradingName_Length_Validation(int lengthOfName, bool isValid)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();

        _command.TradingName = new string('A', lengthOfName);
        var result = await sut.TestValidateAsync(_command);

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.TradingName);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.TradingName)
                .WithErrorMessage(PostOrganisationCommandValidator.TradingNameTooLongErrorMessage);
        }
    }
}
