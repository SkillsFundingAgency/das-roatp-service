using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisatonTypes;
public class GetOrganisationTypesQueryValidatorTests
{
    [TestCase(0, false)]
    [TestCase(4, false)]
    [TestCase(null, true)]
    [TestCase(1, true)]
    [TestCase(2, true)]
    [TestCase(3, true)]
    public void Validate_ProviderTypeId(int? providerTypeId, bool isValid)
    {
        GetOrganisationTypesQueryValidator sut = new();

        var result = sut.TestValidate(new GetOrganisationTypesQuery(providerTypeId));

        if (isValid)
        {
            result.ShouldNotHaveValidationErrorFor(q => q.ProviderTypeId);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(q => q.ProviderTypeId).WithErrorMessage(GetOrganisationTypesQueryValidator.InvalidProviderTypeIdErrorMessage);
        }
    }
}
