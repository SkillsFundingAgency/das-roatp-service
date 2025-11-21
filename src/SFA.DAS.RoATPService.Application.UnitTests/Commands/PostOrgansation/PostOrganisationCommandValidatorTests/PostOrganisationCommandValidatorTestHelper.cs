using System;
using System.Collections.Generic;
using Moq;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.PostOrgansation.PostOrganisationCommandValidatorTests;

public static class PostOrganisationCommandValidatorTestHelper
{
    public const string ValidUserId = "userid";
    public const int ExistingUkprn = 12345678;
    public const int AbsentUkprn = 12345679;
    public const int ValidOrganisationTypeId = 1;
    public const int InvalidOrganisationTypeId = 100;

    public static string ExistingCompanyNumber { get; } = "08849851";
    public const string UnmatchedCompanyNumber = "87654321";
    public const string ExistingCharityNumber = "1089332";
    public const string UnmatchedCharityNumber = "1654321";
    public static PostOrganisationCommandValidator GetValidator() => new PostOrganisationCommandValidator(GetOrganisationRepositoryMock().Object, GetOganisationTypesRepositoryMock().Object);



    public static Mock<IOrganisationsRepository> GetOrganisationRepositoryMock()
    {
        Mock<IOrganisationsRepository> mock = new();
        mock.Setup(r => r.GetOrganisationByUkprn(ExistingUkprn, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new Organisation
        {
            Id = Guid.NewGuid(),
            Ukprn = ExistingUkprn,
            Status = OrganisationStatus.Active,
            OrganisationTypeId = ValidOrganisationTypeId,
            ProviderType = Domain.Common.ProviderType.Main
        });

        mock.Setup(r => r.GetOrganisations(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(new List<Organisation>
        {
            new Organisation {
            Id = Guid.NewGuid(),
            Ukprn = ExistingUkprn,
            Status = OrganisationStatus.Active,
            OrganisationTypeId = ValidOrganisationTypeId,
            ProviderType = Domain.Common.ProviderType.Main,
            CompanyNumber = ExistingCompanyNumber,
            CharityNumber = ExistingCharityNumber
            }
        });
        return mock;
    }

    public static Mock<IOrganisationTypesRepository> GetOganisationTypesRepositoryMock()
    {
        Mock<IOrganisationTypesRepository> mock = new();
        mock.Setup(r => r.GetOrganisationTypes(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(
        [
            new OrganisationType { Id = ValidOrganisationTypeId, Type = "Type1" },
            new OrganisationType { Id = 2, Type = "Type2" }
        ]);
        return mock;
    }
}
