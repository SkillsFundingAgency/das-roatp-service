using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisatonTypes;
public class GetOrganisationTypesQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]

    public async Task Handle_GetOrganisationTypes_ReturnsExpected(
        [Frozen] Mock<IOrganisationTypesRepository> organisationTypesRepositoryMock,
        GetOrganisationTypesQueryHandler sut,
        GetOrganisationTypesQuery query,
        List<OrganisationType> expectedResponse)
    {
        organisationTypesRepositoryMock.Setup(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        var result = await sut.Handle(query, CancellationToken.None);

        result.OrganisationTypes.Should().BeEquivalentTo(expectedResponse, o => o.ExcludingMissingMembers()
            .Excluding(d => d.Organisations));
        organisationTypesRepositoryMock.Verify(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]

    public async Task Handle_GetOrganisationTypes_ReturnsEmpty(
        [Frozen] Mock<IOrganisationTypesRepository> organisationTypesRepositoryMock,
        GetOrganisationTypesQueryHandler sut,
        GetOrganisationTypesQuery query)
    {
        List<OrganisationType> expectedResponse = new();
        organisationTypesRepositoryMock.Setup(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        var result = await sut.Handle(query, CancellationToken.None);

        result.OrganisationTypes.Should().BeEmpty();
    }
}
