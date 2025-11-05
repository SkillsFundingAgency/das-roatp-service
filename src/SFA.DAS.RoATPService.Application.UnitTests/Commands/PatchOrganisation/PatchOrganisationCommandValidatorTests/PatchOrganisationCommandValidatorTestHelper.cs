using System;
using Moq;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PatchOrganisation.PatchOrganisationCommandValidatorTests;

public static class PatchOrganisationCommandValidatorTestHelper
{
    public const string ValidUserId = "userid";
    public const int ValidUkprn = 12345678;
    public static PatchOrganisationCommandValidator GetValidator() => new PatchOrganisationCommandValidator(GetRemovedReasonsRepositoryMock().Object, GetOganisationTypesRepositoryMock().Object, GetOrganisationRepositoryMock(OrganisationStatus.Active).Object);

    public static Mock<IOrganisationTypesRepository> GetOganisationTypesRepositoryMock()
    {
        Mock<IOrganisationTypesRepository> mock = new();
        mock.Setup(r => r.GetOrganisationTypes(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(
        [
            new OrganisationType { Id = 1, Type = "Type1" },
            new OrganisationType { Id = 2, Type = "Type2" }
        ]);
        return mock;
    }

    public static Mock<IRemovedReasonsRepository> GetRemovedReasonsRepositoryMock()
    {
        Mock<IRemovedReasonsRepository> mock = new();
        mock.Setup(r => r.GetAllRemovedReasons(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(
        [
            new RemovedReason { Id = 1, Description = "reason 1"},
            new RemovedReason { Id = 2, Description = "reason 2"}
        ]);
        return mock;
    }

    public static Mock<IOrganisationsRepository> GetOrganisationRepositoryMock(OrganisationStatus organisationStatus)
    {
        Mock<IOrganisationsRepository> mock = new();
        mock.Setup(r => r.GetOrganisationByUkprn(It.IsAny<int>(), It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new Organisation
        {
            Id = Guid.NewGuid(),
            Ukprn = ValidUkprn,
            Status = organisationStatus,
            OrganisationTypeId = 1,
            ProviderType = ProviderType.Main
        });
        return mock;
    }
}
