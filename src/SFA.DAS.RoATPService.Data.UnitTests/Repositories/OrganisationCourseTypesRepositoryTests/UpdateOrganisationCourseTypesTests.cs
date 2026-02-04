using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories.OrganisationCourseTypesRepositoryTests;

public class UpdateOrganisationCourseTypesTests
{
    [Test]
    public async Task UpdateOrganisationCourseTypes_RemovesAndAddsCourses()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new RoatpDataContext(options);

        var organisationId = Guid.NewGuid();
        Organisation organisation = new()
        {
            Id = organisationId,
            LegalName = "Test Organisation",
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedBy = "InitialUser"
        };
        context.Organisations.Add(organisation);

        // Seed a short course type and a non-short course type
        var standardCourseType = new CourseType { Id = 1, Name = "Apprenticeship" };
        var shortCourseUnitType = new CourseType { Id = 2, Name = "ShortCourse" };

        context.CourseTypes.AddRange(standardCourseType, shortCourseUnitType);

        // Seed existing OrganisationCourseTypes
        context.OrganisationCourseTypes.AddRange(
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = standardCourseType.Id, CourseType = standardCourseType }
        );
        await context.SaveChangesAsync();

        var sut = new OrganisationCourseTypesRepository(context);
        var newUserId = "TestUserId";

        // Act
        var newShortCourseIds = new[] { shortCourseUnitType.Id }; // Only keep short course bootcamp type 
        await sut.UpdateOrganisationCourseTypes(organisation, newShortCourseIds, newUserId, CancellationToken.None);

        // Assert
        var orgCourseTypes = await context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId).ToListAsync();
        Assert.Multiple(() =>
        {
            Assert.That(orgCourseTypes, Has.Count.EqualTo(1));
            Assert.That(orgCourseTypes.Exists(o => o.CourseTypeId == shortCourseUnitType.Id), Is.True);
            Assert.That(context.Audits.CountAsync().Result, Is.EqualTo(1));
            Assert.That(context.Organisations.First().UpdatedAt.GetValueOrDefault().Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(context.Organisations.First().UpdatedBy, Is.EqualTo(newUserId));
        });

    }
}
