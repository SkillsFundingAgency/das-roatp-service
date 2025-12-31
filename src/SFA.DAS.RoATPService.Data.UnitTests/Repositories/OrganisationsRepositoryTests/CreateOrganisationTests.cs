using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.AuditModels;
using SFA.DAS.RoATPService.Domain.Entities;
using Organisation = SFA.DAS.RoATPService.Domain.Entities.Organisation;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories.OrganisationsRepositoryTests;

public class CreateOrganisationTests
{
    [Test]
    public async Task CreateOrganisation_OrganisationAdded()
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
        OrganisationStatusEvent organisationStatusEvent = new OrganisationStatusEvent();

        organisation.LegalName = newOrganisationName;
        organisation.UpdatedAt = DateTime.UtcNow;
        organisation.UpdatedBy = userId;

        var sut = new OrganisationsRepository(context);

        await sut.CreateOrganisation(organisation, audit, organisationStatusEvent, cancellationToken);
        Organisation addedOrganisation = await context.Organisations.FirstAsync(s => s.Id == organisationId);
        addedOrganisation.LegalName.Should().Be(newOrganisationName);
        addedOrganisation.UpdatedBy.Should().Be(userId);
        addedOrganisation.UpdatedAt!.Value.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Test]
    public async Task CreateOrganisation_AuditAdded()
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

        Audit audit = new Audit
        {
            AuditData = new AuditData
            {
                FieldChanges = new List<AuditLogEntry>(),
                OrganisationId = organisationId
            },
            OrganisationId = organisationId
        };

        organisation.LegalName = newOrganisationName;

        var sut = new OrganisationsRepository(context);

        await sut.CreateOrganisation(organisation, audit, new OrganisationStatusEvent(), cancellationToken);
        context.Audits.CountAsync().Result.Should().Be(1);
        var actual = await context.Audits.FirstAsync();
        actual.OrganisationId.Should().Be(organisationId);
        actual.AuditData.FieldChanges.Should().BeEquivalentTo(new List<AuditLogEntry>());
        actual.AuditData.OrganisationId.Should().Be(organisationId);
    }

    [Test]
    public async Task CreateOrganisation_StatusEventAdded()
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

        organisation.LegalName = newOrganisationName;

        var sut = new OrganisationsRepository(context);

        await sut.CreateOrganisation(organisation, audit, organisationStatusEvent, cancellationToken);
        var eventAdded = await context.OrganisationStatusEvents.FirstAsync();
        context.OrganisationStatusEvents.CountAsync().Result.Should().Be(1);
        eventAdded.OrganisationStatus.Should().Be(organisation.Status);
        eventAdded.Ukprn.Should().Be(organisation.Ukprn);
    }
}
