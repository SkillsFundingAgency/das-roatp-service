using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories.OrganisationCourseTypesRepositoryTests;

public class DeleteOrganisationShortCourseTypesTests
{
    [Test]
    public async Task DeleteOrganisationShortCourseTypes_DeletesShortCourses()
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
        var standardCourseType = new CourseType { Id = 1, Name = "StandardCourse" };
        var shortCourseType1 = new CourseType { Id = 2, Name = "ShortCourse1" };

        context.CourseTypes.AddRange(standardCourseType, shortCourseType1);

        // Seed existing OrganisationCourseTypes
        context.OrganisationCourseTypes.AddRange(
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = standardCourseType.Id, CourseType = standardCourseType },
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = shortCourseType1.Id, CourseType = shortCourseType1 }
        );

        await context.SaveChangesAsync();

        var sut = new OrganisationCourseTypesRepository(context);

        var newUserId = "TestUserId";

        // Act
        await sut.DeleteOrganisationShortCourseTypes(organisation, newUserId, CancellationToken.None);

        // Assert
        var organisationCourseTypes = await context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId).ToListAsync();
        Assert.Multiple(() =>
        {
            Assert.That(organisationCourseTypes, Has.Count.EqualTo(1));
            Assert.That(organisationCourseTypes.Exists(o => o.CourseTypeId == standardCourseType.Id), Is.True);
            Assert.That(organisationCourseTypes.Exists(o => o.CourseTypeId == shortCourseType1.Id), Is.False);
            Assert.That(context.Audits.CountAsync().Result, Is.EqualTo(1));
            Assert.That(context.Organisations.First().UpdatedAt.GetValueOrDefault().Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(context.Organisations.First().UpdatedBy, Is.EqualTo(newUserId));
        });

    }
}