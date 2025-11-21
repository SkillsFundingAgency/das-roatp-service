using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;



[TestFixture]
public class PostOrganisationCommandValidatorLegalNameTests
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
    public async Task Validate_LegalName_IsValid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.LegalName);
    }

    [Test]
    public async Task Validate_LegalName_IsNotPresent()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.LegalName = "";
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.LegalName).WithErrorMessage(PostOrganisationCommandValidator.LegalNameIsRequiredErrorMessage);
    }

    [TestCase(199, true)]
    [TestCase(200, true)]
    [TestCase(201, false)]
    public async Task Validate_LegalName_Length_Validation(int lengthOfLegalName, bool isValid)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();

        _command.LegalName = new string('A', lengthOfLegalName);
        var result = await sut.TestValidateAsync(_command);

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(c => c.LegalName);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(c => c.LegalName)
                .WithErrorMessage(PostOrganisationCommandValidator.LegalNameTooLongErrorMessage);
        }
    }
}
