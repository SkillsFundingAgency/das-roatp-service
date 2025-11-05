using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetAllProviderTypes;

public class ProviderTypeModelTests
{
    [Test, RecursiveMoqAutoData]
    public void ConvertsFromDomainEntityCorrectly(ProviderType domainEntity)
    {
        // Act
        ProviderTypeModel model = domainEntity;
        // Assert
        Assert.AreEqual(domainEntity.Id, model.Id);
        Assert.AreEqual(domainEntity.Name, model.Type);
        Assert.AreEqual(domainEntity.Description, model.Description);
    }
}
