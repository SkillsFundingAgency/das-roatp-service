using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories.OrganisationsRepositoryTests;
public class UpdateOrganisationTests
{
    [Test]
    public async Task UpdateOrganisation_OrganisationDetailsUpdated()
    {
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new RoatpDataContext(options);

        CancellationToken cancellationToken = new CancellationToken();
        var newOrganisationName = "new name";
        var userId = "user name";

        var organisationId = Guid.NewGuid();

        Organisation organisation = new()
        {
            Id = organisationId,
            LegalName = "Test Organisation",
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedBy = "InitialUser"
        };

        Audit audit = new Audit();

        context.Organisations.Add(organisation);

        await context.SaveChangesAsync(cancellationToken);
        organisation.LegalName = newOrganisationName;
        organisation.UpdatedAt = DateTime.UtcNow;
        organisation.UpdatedBy = userId;

        var sut = new OrganisationsRepository(context);

        await sut.UpdateOrganisation(organisation, audit, null, cancellationToken);
        var updatedOrganisation = context.Organisations.First(s => s.Id == organisationId);
        updatedOrganisation.LegalName.Should().Be(newOrganisationName);
        updatedOrganisation.UpdatedBy.Should().Be(userId);
        updatedOrganisation.UpdatedAt!.Value.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Test]
    public async Task UpdateOrganisation_AuditAdded()
    {
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new RoatpDataContext(options);

        CancellationToken cancellationToken = new CancellationToken();
        var newOrganisationName = "new name";

        var organisationId = Guid.NewGuid();

        Organisation organisation = new()
        {
            Id = organisationId,
            LegalName = "Test Organisation",
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedBy = "InitialUser"
        };

        Audit audit = new Audit();

        context.Organisations.Add(organisation);

        await context.SaveChangesAsync(cancellationToken);
        organisation.LegalName = newOrganisationName;
        var expectedAuditCount = context.Audits.Count() + 1;

        var sut = new OrganisationsRepository(context);

        await sut.UpdateOrganisation(organisation, audit, null, cancellationToken);
        expectedAuditCount.Should().Be(context.Audits.Count());
    }

    [Test]
    public async Task UpdateOrganisation_StatusEventAdded()
    {
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new RoatpDataContext(options);

        CancellationToken cancellationToken = new CancellationToken();
        var newOrganisationName = "new name";
        var organisationStatusEvent = new OrganisationStatusEvent();

        var organisationId = Guid.NewGuid();

        Organisation organisation = new()
        {
            Id = organisationId,
            LegalName = "Test Organisation",
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedBy = "InitialUser"
        };

        Audit audit = new Audit();

        context.Organisations.Add(organisation);

        await context.SaveChangesAsync(cancellationToken);
        organisation.LegalName = newOrganisationName;

        var expectedOrganisationStatusEventCount = context.OrganisationStatusEvents.Count() + 1;

        var sut = new OrganisationsRepository(context);

        await sut.UpdateOrganisation(organisation, audit, organisationStatusEvent, cancellationToken);
        expectedOrganisationStatusEventCount.Should().Be(context.OrganisationStatusEvents.Count());
    }
}
