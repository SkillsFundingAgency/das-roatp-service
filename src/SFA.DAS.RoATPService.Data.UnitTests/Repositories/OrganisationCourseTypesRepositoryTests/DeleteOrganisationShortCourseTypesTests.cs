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

        // Seed a short course type and a non-short course type
        var standardCourseType = new CourseType { Id = 1, Name = "StandardCourse", LearningType = LearningType.Standard };
        var shortCourseType1 = new CourseType { Id = 2, Name = "ShortCourse1", LearningType = LearningType.ShortCourse };
        var shortCourseType2 = new CourseType { Id = 3, Name = "ShortCourse2", LearningType = LearningType.ShortCourse };

        context.CourseTypes.AddRange(standardCourseType, shortCourseType1, shortCourseType2);

        // Seed existing OrganisationCourseTypes
        context.OrganisationCourseTypes.AddRange(
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = standardCourseType.Id, CourseType = standardCourseType },
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = shortCourseType1.Id, CourseType = shortCourseType1 },
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = shortCourseType2.Id, CourseType = shortCourseType2 }
        );

        await context.SaveChangesAsync();

        var sut = new OrganisationCourseTypesRepository(context);

        // Act
        await sut.DeleteOrganisationShortCourseTypes(organisationId, "TestUserId", CancellationToken.None);

        // Assert
        var organisationCourseTypes = context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(organisationCourseTypes, Has.Count.EqualTo(1));
            Assert.That(organisationCourseTypes.Exists(o => o.CourseTypeId == standardCourseType.Id), Is.True);
            Assert.That(organisationCourseTypes.Exists(o => o.CourseTypeId == shortCourseType1.Id), Is.False);
            Assert.That(organisationCourseTypes.Exists(o => o.CourseTypeId == shortCourseType2.Id), Is.False);
        });

        Assert.That(context.Audits.CountAsync().Result, Is.EqualTo(1));
    }
}