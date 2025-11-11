using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories.OrganisationsRepositoryTests;
public class UpdateOrganisationTests
{
    [Test]
    public async Task UpdateOrganisation_NoStatusEvent_NoRemovedShortCourses()
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
        context.OrganisationCourseTypes.Add(new OrganisationCourseType { CourseType = new CourseType { Id = 1, IsActive = true, LearningType = LearningType.Standard } });

        await context.SaveChangesAsync(cancellationToken);
        organisation.LegalName = newOrganisationName;

        var expectedOrganisationStatusEventsCount = context.OrganisationStatusEvents.Count();
        var expectedAuditCount = context.Audits.Count() + 1;

        var sut = new OrganisationsRepository(context);

        await sut.UpdateOrganisation(organisation, audit, null, false, userId, cancellationToken);

        var updatedOrganisation = context.Organisations.First(s => s.Id == organisationId);

        expectedOrganisationStatusEventsCount.Should().Be(context.OrganisationStatusEvents.Count());
        expectedAuditCount.Should().Be(context.Audits.Count());
        updatedOrganisation.LegalName.Should().Be(newOrganisationName);
        context.OrganisationStatusEvents.Count().Should().Be(0);
        context.OrganisationCourseTypes.Count().Should().Be(1);
    }

    [Test]
    public async Task UpdateOrganisation_StatusEventAdded_NoRemovedShortCourses()
    {
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new RoatpDataContext(options);

        CancellationToken cancellationToken = new CancellationToken();
        var newOrganisationName = "new name";
        var userId = "user name";
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

        var expectedAuditCount = context.Audits.Count() + 1;
        var expectedOrganisationStatusEventCount = context.OrganisationStatusEvents.Count() + 1;

        var sut = new OrganisationsRepository(context);

        await sut.UpdateOrganisation(organisation, audit, organisationStatusEvent, false, userId, cancellationToken);

        var updatedOrganisation = context.Organisations.First(s => s.Id == organisationId);

        expectedOrganisationStatusEventCount.Should().Be(context.OrganisationStatusEvents.Count());
        expectedAuditCount.Should().Be(context.Audits.Count());
        updatedOrganisation.LegalName.Should().Be(newOrganisationName);
    }


    [Test]
    public async Task UpdateOrganisation_StatusEventAdded_RemoveShortCourses()
    {
        var removeShortCourses = true;
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new RoatpDataContext(options);

        CancellationToken cancellationToken = new CancellationToken();
        var newOrganisationName = "new name";
        var userId = "user name";
        var organisationStatusEvent = new OrganisationStatusEvent();


        var organisationId = Guid.NewGuid();

        Organisation organisation = new()
        {
            Id = organisationId,
            LegalName = "Test Organisation",
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedBy = "InitialUser",
            ProviderType = ProviderType.Main
        };

        Audit audit = new Audit();

        context.Organisations.Add(organisation);
        context.OrganisationCourseTypes.Add(
            new OrganisationCourseType
            {
                OrganisationId = organisationId,
                CourseType = new CourseType { Id = 1, IsActive = true, LearningType = LearningType.Standard }
            });
        context.OrganisationCourseTypes.Add(
            new OrganisationCourseType
            {
                OrganisationId = organisationId,
                CourseType = new CourseType { Id = 2, IsActive = true, LearningType = LearningType.ShortCourse }
            });


        await context.SaveChangesAsync(cancellationToken);
        organisation.LegalName = newOrganisationName;
        organisation.ProviderType = ProviderType.Supporting;

        var expectedAuditCount = context.Audits.Count() + 2;
        var expectedOrganisationStatusEventCount = context.OrganisationStatusEvents.Count() + 1;
        var expectedOrganisationCourseTypesCount = context.OrganisationCourseTypes.Count() - 1;
        var sut = new OrganisationsRepository(context);

        await sut.UpdateOrganisation(organisation, audit, organisationStatusEvent, removeShortCourses, userId, cancellationToken);

        var updatedOrganisation = context.Organisations.First(s => s.Id == organisationId);

        expectedOrganisationStatusEventCount.Should().Be(context.OrganisationStatusEvents.Count());
        expectedAuditCount.Should().Be(context.Audits.Count());
        updatedOrganisation.LegalName.Should().Be(newOrganisationName);
        expectedOrganisationCourseTypesCount.Should().Be(context.OrganisationCourseTypes.Count());
    }
}
