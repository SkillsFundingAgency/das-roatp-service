using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Common;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories;

public class OrganisationStatusEventsRepositoryTests
{
    private List<OrganisationStatusEvent> _events = [];

    [SetUp]
    public void Arrange()
    {
        _events = new List<OrganisationStatusEvent>
        {
            new OrganisationStatusEvent { Id = 1, Ukprn = 10000001, OrganisationStatus = OrganisationStatus.Active, CreatedOn = DateTime.UtcNow  },
            new OrganisationStatusEvent { Id = 2, Ukprn = 10000001, OrganisationStatus = OrganisationStatus.Removed, CreatedOn = DateTime.UtcNow },
            new OrganisationStatusEvent { Id = 3, Ukprn = 10000003, OrganisationStatus = OrganisationStatus.Active, CreatedOn = DateTime.UtcNow },
            new OrganisationStatusEvent { Id = 4, Ukprn = 10000004, OrganisationStatus = OrganisationStatus.ActiveNoStarts, CreatedOn = DateTime.UtcNow},
            new OrganisationStatusEvent { Id = 5, Ukprn = 10000005, OrganisationStatus = OrganisationStatus.Active, CreatedOn = DateTime.UtcNow }
        };
    }

    [Test]
    public async Task GetOrganisationStatusEvents_ReturnsAllEvents()
    {
        // Arrange
        await using var context = GetDataContext();
        var sut = new OrganisationStatusEventsRepository(context);
        // Act
        var result = await sut.GetOrganisationStatusEvents(0, 1000, 1, CancellationToken.None);
        // Assert
        Assert.That(result.Count, Is.EqualTo(_events.Count));
        foreach (var evt in _events)
        {
            Assert.That(result.Any(r => r.Id == evt.Id && r.Ukprn == evt.Ukprn && r.OrganisationStatus == evt.OrganisationStatus), Is.True);
        }
    }

    [Test]
    public async Task GetOrganisationStatusEvents_WithPaging_ReturnsPagedEvents()
    {
        // Arrange
        await using var context = GetDataContext();
        var sut = new OrganisationStatusEventsRepository(context);
        // Act
        var result = await sut.GetOrganisationStatusEvents(1, 2, 1, CancellationToken.None);
        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(2));
        Assert.That(result[1].Id, Is.EqualTo(3));
    }

    [Test]
    public async Task GetOrganisationStatusHistory_ReturnsEventsForUkprn()
    {
        // Arrange
        await using var context = GetDataContext();
        var sut = new OrganisationStatusEventsRepository(context);
        var ukprn = 10000001;
        // Act
        var result = await sut.GetOrganisationStatusEventsByUkprn(ukprn, CancellationToken.None);
        // Assert
        var expectedEvents = _events.Where(e => e.Ukprn == ukprn).ToList();
        Assert.That(result.Count, Is.EqualTo(expectedEvents.Count));
        foreach (var evt in expectedEvents)
        {
            Assert.That(result.Any(r => r.Id == evt.Id && r.Ukprn == evt.Ukprn && r.OrganisationStatus == evt.OrganisationStatus), Is.True);
        }
    }

    private RoatpDataContext GetDataContext()
    {
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new RoatpDataContext(options);
        context.OrganisationStatusEvents.AddRange(_events);
        context.SaveChanges();
        return context;
    }
}
