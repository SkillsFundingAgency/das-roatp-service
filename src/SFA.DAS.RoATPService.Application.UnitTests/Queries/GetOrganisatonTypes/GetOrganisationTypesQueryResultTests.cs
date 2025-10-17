using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisatonTypes;
public class GetOrganisationTypesQueryResultTests
{
    [Test, RecursiveMoqAutoData]

    public void GetOrganisationTypesQueryResult_MapsDataToExpected(
        OrganisationType organisationType)
    {
        OrganisationTypeSummary organisationTypeSummary = organisationType;
        GetOrganisationTypesQueryResult sut = new() { OrganisationTypes = [organisationTypeSummary] };

        var result = sut.OrganisationTypes.First();

        result.Id.Should().Be(organisationType.Id);
        result.Description.Should().Be(organisationType.Type);
    }
}
