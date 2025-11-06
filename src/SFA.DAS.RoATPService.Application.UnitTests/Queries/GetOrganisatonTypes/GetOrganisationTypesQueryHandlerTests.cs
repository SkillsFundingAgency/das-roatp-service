using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisatonTypes;
public class GetOrganisationTypesQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_NoProviderType_ReturnsAllOrganisationTypes(
        [Frozen] Mock<IOrganisationTypesRepository> organisationTypesRepositoryMock,
        [Frozen] Mock<IProviderTypeOrganisationTypesRepository> providerTypeOrganisationTypesRepositoryMock,
        GetOrganisationTypesQueryHandler sut,
        List<OrganisationType> expectedResponse)
    {
        GetOrganisationTypesQuery query = new(null);
        organisationTypesRepositoryMock.Setup(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        ValidatedResponse<GetOrganisationTypesQueryResult> result = await sut.Handle(query, CancellationToken.None);

        result.Result.OrganisationTypes.Should().BeEquivalentTo(expectedResponse, o => o.ExcludingMissingMembers()
            .Excluding(d => d.Organisations));
        organisationTypesRepositoryMock.Verify(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>()), Times.Once);
        providerTypeOrganisationTypesRepositoryMock.Verify(m => m.GetOrganisationTypeByProviderTypeId(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_NoProviderType_EmptyList_ReturnsOk(
        [Frozen] Mock<IOrganisationTypesRepository> organisationTypesRepositoryMock,
        GetOrganisationTypesQueryHandler sut)
    {
        GetOrganisationTypesQuery query = new(null);
        List<OrganisationType> expectedResponse = new();
        organisationTypesRepositoryMock.Setup(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);

        ValidatedResponse<GetOrganisationTypesQueryResult> result = await sut.Handle(query, CancellationToken.None);

        result.Result.OrganisationTypes.Should().BeEmpty();
        organisationTypesRepositoryMock.Verify(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_WithProviderType_ReturnsFilteredOrganisationTypes(
        [Frozen] Mock<IProviderTypeOrganisationTypesRepository> providerTypeOrganisationTypesRepositoryMock,
        [Frozen] Mock<IOrganisationTypesRepository> organisationTypesRepositoryMock,
        GetOrganisationTypesQueryHandler sut,
        List<OrganisationType> expectedResponse)
    {
        int providerTypeId = 2;
        GetOrganisationTypesQuery query = new(providerTypeId);
        providerTypeOrganisationTypesRepositoryMock.Setup(m => m.GetOrganisationTypeByProviderTypeId(providerTypeId, It.IsAny<CancellationToken>())).ReturnsAsync(expectedResponse);
        ValidatedResponse<GetOrganisationTypesQueryResult> result = await sut.Handle(query, CancellationToken.None);
        result.Result.OrganisationTypes.Should().BeEquivalentTo(expectedResponse, o => o.ExcludingMissingMembers()
            .Excluding(d => d.Organisations));
        providerTypeOrganisationTypesRepositoryMock.Verify(m => m.GetOrganisationTypeByProviderTypeId(providerTypeId, It.IsAny<CancellationToken>()), Times.Once);
        organisationTypesRepositoryMock.Verify(m => m.GetOrganisationTypes(It.IsAny<CancellationToken>()), Times.Never);
    }
}
