using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories.AuditsRepositoryTests;
public class GetLastUpdatedDateForOrganisationTests
{
    [Test]
    public async Task GetLastUpdatedDateForOrganisation_ReturnsLatestUpdatedAt()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var organisationId = Guid.NewGuid();
        var audit1 = new Audit { OrganisationId = organisationId, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) };
        var expectedAudit = new Audit { OrganisationId = organisationId, UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) };
        var auditOther = new Audit { OrganisationId = Guid.NewGuid(), UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) };

        using (var context = new RoatpDataContext(options))
        {
            context.Audits.AddRange(audit1, expectedAudit, auditOther);
            context.SaveChanges();
        }

        using (var context = new RoatpDataContext(options))
        {
            var repo = new AuditsRepository(context);
            // Act
            var result = await repo.GetLastUpdatedDateForOrganisation(organisationId, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(expectedAudit.UpdatedAt));
        }
    }

    [Test]
    public async Task GetLastUpdatedDateForOrganisation_NoAudits_ReturnsNull()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var organisationId = Guid.NewGuid();

        using (var context = new RoatpDataContext(options))
        {
            // No audits added
        }

        using (var context = new RoatpDataContext(options))
        {
            var repo = new AuditsRepository(context);
            // Act
            var result = await repo.GetLastUpdatedDateForOrganisation(organisationId, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
