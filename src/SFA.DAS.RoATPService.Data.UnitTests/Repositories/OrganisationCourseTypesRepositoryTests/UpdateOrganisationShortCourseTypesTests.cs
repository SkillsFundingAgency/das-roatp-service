using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Data.Repositories;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Data.UnitTests.Repositories.OrganisationCourseTypesRepositoryTests;

public class UpdateOrganisationShortCourseTypesTests
{
    [Test]
    public async Task UpdateOrganisationShortCourseTypes_RemovesAndAddsShortCourses()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<RoatpDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new RoatpDataContext(options);

        var organisationId = Guid.NewGuid();

        // Seed a short course type and a non-short course type
        var standardCourseType = new CourseType { Id = 1, Name = "Apprenticeship", LearningType = LearningType.Standard };
        var shortCourseUnitType = new CourseType { Id = 2, Name = "Unit", LearningType = LearningType.ShortCourse };
        var shortCourseBootcampType = new CourseType { Id = 3, Name = "Bootcamp", LearningType = LearningType.ShortCourse };

        context.CourseTypes.AddRange(standardCourseType, shortCourseUnitType, shortCourseBootcampType);

        // Seed existing OrganisationCourseTypes
        context.OrganisationCourseTypes.AddRange(
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = standardCourseType.Id, CourseType = standardCourseType },
            new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = shortCourseUnitType.Id, CourseType = shortCourseUnitType }
        );
        await context.SaveChangesAsync();

        var sut = new OrganisationCourseTypesRepository(context);

        // Act
        var newShortCourseIds = new[] { shortCourseBootcampType.Id }; // Only keep short course bootcamp type 
        await sut.UpdateOrganisationShortCourseTypes(organisationId, newShortCourseIds, "userid", CancellationToken.None);

        // Assert
        var orgCourseTypes = context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(orgCourseTypes.Count, Is.EqualTo(2));
            Assert.That(orgCourseTypes.Exists(o => o.CourseTypeId == standardCourseType.Id), Is.True);
            Assert.That(orgCourseTypes.Exists(o => o.CourseTypeId == shortCourseBootcampType.Id), Is.True);
        });
    }
}
