using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetAllProviderTypes;

public class GetAllProviderTypesQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handler_ReturnsProviderTypes(
        [Frozen] Mock<IProviderTypesRepository> providerTypesRepositoryMock,
        GetAllProviderTypesQueryHandler sut,
        List<ProviderType> providerTypes)
    {
        //Arrange
        providerTypesRepositoryMock.Setup(x => x.GetAll(default)).ReturnsAsync(providerTypes);
        //Act
        var actual = await sut.Handle(new GetAllProviderTypesQuery(), default);
        //Assert
        Assert.That(actual.ProviderTypes.Count(), Is.EqualTo(providerTypes.Count));
    }
}
