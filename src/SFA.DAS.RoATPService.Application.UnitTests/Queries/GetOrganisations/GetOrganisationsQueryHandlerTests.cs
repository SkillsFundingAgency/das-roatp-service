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
        List<Organisation> expectedOrganisations,
        CancellationToken cancellationToken)
    {
        GetOrganisationsQuery query = new(null);
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

    [Test, RecursiveMoqAutoData]
    public async Task Handle_NoSearchTerm_GetAllOrganisations(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        GetOrganisationsQueryHandler sut,
        List<Organisation> expectedOrganisations,
        CancellationToken cancellationToken)
    {
        GetOrganisationsQuery query = new(null);
        organisationRepositoryMock.Setup(x => x.GetOrganisations(cancellationToken)).ReturnsAsync(expectedOrganisations);

        await sut.Handle(query, cancellationToken);

        organisationRepositoryMock.Verify(x => x.GetOrganisations(It.IsAny<CancellationToken>()), Times.Once);
        organisationRepositoryMock.Verify(x => x.GetOrganisationsBySearchTerm(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_NoSearchTerm_GetAllOrganisations(
        [Frozen] Mock<IOrganisationsRepository> organisationRepositoryMock,
        GetOrganisationsQueryHandler sut,
        string searchTerm,
        List<Organisation> expectedOrganisations,
        CancellationToken cancellationToken)
    {
        GetOrganisationsQuery query = new(searchTerm);
        organisationRepositoryMock.Setup(x => x.GetOrganisationsBySearchTerm(query.SearchTerm, cancellationToken)).ReturnsAsync(expectedOrganisations);

        await sut.Handle(query, cancellationToken);

        organisationRepositoryMock.Verify(x => x.GetOrganisationsBySearchTerm(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        organisationRepositoryMock.Verify(x => x.GetOrganisations(It.IsAny<CancellationToken>()), Times.Never);
    }
}
