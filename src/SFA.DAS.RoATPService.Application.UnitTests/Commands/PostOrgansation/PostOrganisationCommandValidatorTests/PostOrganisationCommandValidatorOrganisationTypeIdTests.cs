using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;

[TestFixture]
public class PostOrganisationCommandValidatorOrganisationTypeIdTests
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

    [Test]
    public async Task Validate_OrganisationTypeId_IsValid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();

        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.OrganisationTypeId);
    }

    [Test]
    public async Task Validate_OrganisationTypeId_Null_IsInvalid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.OrganisationTypeId = null;

        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationTypeId).WithErrorMessage(PostOrganisationCommandValidator.OrganisationTypeIdIsRequiredErrorMessage);
    }

    [Test]
    public async Task Validate_OrganisationTypeId_Invalid()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        _command.OrganisationTypeId = PostOrganisationCommandValidatorTestHelper.InvalidOrganisationTypeId;
        var result = await sut.TestValidateAsync(_command);
        result.ShouldHaveValidationErrorFor(c => c.OrganisationTypeId).WithErrorMessage(PostOrganisationCommandValidator.OrganisationTypeIdShouldBeValidErrorMessage);
    }
}
