using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.Commands.UpsertOrganisation;

namespace SFA.DAS.RoATPService.Api.UnitTests.Models;

public class UpsertOrganisationModelTests
{
    [Test]
    public void Operator_ConvertsToCommand()
    {
        // Arrange
        var model = new CreateOrganisationModel
        {
            Ukprn = 12345678,
            LegalName = "Test Legal Name",
            TradingName = "Test Trading Name",
            CompanyNumber = "12345678",
            CharityNumber = "87654321",
            ProviderType = SFA.DAS.RoATPService.Domain.Common.ProviderType.Main,
            OrganisationTypeId = 1,
            RequestingUserId = "user-123"
        };
        // Act
        UpsertOrganisationCommand command = model;
        // Assert
        Assert.That(model.Ukprn, Is.EqualTo(command.Ukprn));
        Assert.That(model.LegalName, Is.EqualTo(command.LegalName));
        Assert.That(model.TradingName, Is.EqualTo(command.TradingName));
        Assert.That(model.CompanyNumber, Is.EqualTo(command.CompanyNumber));
        Assert.That(model.CharityNumber, Is.EqualTo(command.CharityNumber));
        Assert.That(model.ProviderType, Is.EqualTo(command.ProviderType));
        Assert.That(model.OrganisationTypeId, Is.EqualTo(command.OrganisationTypeId));
        Assert.That(model.RequestingUserId, Is.EqualTo(command.RequestingUserId));
    }
}
