using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetCourseTypes;
public class GetAllCourseTypesResultTests
{
    [Test, RecursiveMoqAutoData]

    public void GetAllCourseTypesResult_MapsCourseTypeDataCorrectly(CourseType courseType)
    {
        // Arrange
        var sut = new GetAllCourseTypesResult
        {
            CourseTypes = new List<CourseTypeSummary> { courseType }
        };
        ;
        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(courseType.Id, Is.EqualTo(sut.CourseTypes.FirstOrDefault().Id));
            Assert.That(courseType.Name, Is.EqualTo(sut.CourseTypes.FirstOrDefault().Name));
            Assert.That(courseType.LearningType, Is.EqualTo(sut.CourseTypes.FirstOrDefault().LearningType));
        });
    }
}