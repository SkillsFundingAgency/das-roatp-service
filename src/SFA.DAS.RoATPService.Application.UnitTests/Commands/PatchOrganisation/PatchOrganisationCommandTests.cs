using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Domain.Common;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PatchOrganisation;
public class PatchOrganisationCommandTests
{
    [Test]
    public void ConvertsOrganisationToPatchOrganisationModel()
    {
        //Arrange
        var organisation = new Domain.Entities.Organisation
        {
            Status = Domain.Entities.OrganisationStatus.Active,
            RemovedReasonId = null,
            ProviderType = ProviderType.Main,
            OrganisationTypeId = 1
        };
        //Act
        PatchOrganisationModel model = organisation;
        //Assert
        Assert.AreEqual(organisation.Status, model.Status);
        Assert.AreEqual(organisation.RemovedReasonId, model.RemovedReasonId);
        Assert.AreEqual(organisation.ProviderType, model.ProviderType);
        Assert.AreEqual(organisation.OrganisationTypeId, model.OrganisationTypeId);
    }
}
