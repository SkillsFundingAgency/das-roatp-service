using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Application.Common.Validators;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;



[TestFixture]
public class PostOrganisationCommandValidatorUkprnTests
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

    [TestCase(1234567)]
    [TestCase(123456789)]
    [TestCase(23456789)]
    public async Task Validate_Ukprn_ShouldBeCorrectFormat(int ukprn)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();

        _command.Ukprn = ukprn;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(UkprnValidator.UkprnFormatValidationMessage);
    }

    [Test]
    public async Task Validate_Ukprn_IsNotAlreadyPresent()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.Ukprn);
    }

    [Test]
    public async Task Validate_Ukprn_IsAlreadyPresent()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.Ukprn = PostOrganisationCommandValidatorTestHelper.ExistingUkprn;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.Ukprn).WithErrorMessage(PostOrganisationCommandValidator.UkprnAlreadyPresentErrorMessage);
    }
}
