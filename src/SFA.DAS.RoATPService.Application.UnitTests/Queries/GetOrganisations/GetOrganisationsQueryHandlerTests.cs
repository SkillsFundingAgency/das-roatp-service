using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisations;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisations;

[TestFixture]
public class GetOrganisationsQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_OrganisationsFound_ReturnsOrganisations(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        GetOrganisationsQueryHandler sut,
        GetOrganisationsQuery query,
        List<Organisation> expectedOrganisations,
        CancellationToken cancellationToken)
    {
        organisationRepositoryMock.Setup(x => x.GetOrganisations(cancellationToken)).ReturnsAsync(expectedOrganisations);
        GetOrganisationsQueryResult actual = await sut.Handle(query, cancellationToken);

        actual.Organisations.Should().BeEquivalentTo(
            expectedOrganisations,
            o => o.ExcludingMissingMembers()
                .Excluding(o => o.OrganisationCourseTypes)
                .Excluding(o => o.OrganisationTypeId)
                .Excluding(x => x.OrganisationType)
        );
    }
}
