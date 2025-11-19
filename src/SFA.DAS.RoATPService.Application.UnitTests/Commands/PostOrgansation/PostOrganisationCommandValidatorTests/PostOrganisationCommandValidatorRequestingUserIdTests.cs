using System.Threading.Tasks;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;

[TestFixture]
public class PostOrganisationCommandValidatorRequestingUserIdTests
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
    public async Task Validate_RequestingUserId_IsPresent()
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();
        var result = await sut.TestValidateAsync(_command);
        result.ShouldNotHaveValidationErrorFor(c => c.RequestingUserId);
    }

    [TestCase("")]
    [TestCase(null)]
    public async Task Validate_RequestingUserId_NotSet(string userId)
    {
        var sut = PostOrganisationCommandValidatorTestHelper.GetValidator();

        _command.RequestingUserId = userId;
        var result = await sut.TestValidateAsync(_command);

        result.ShouldHaveValidationErrorFor(c => c.RequestingUserId)
                .WithErrorMessage(PostOrganisationCommandValidator.RequestingUserIdRequiredErrorMessage);

    }
}
