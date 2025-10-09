using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetCourseTypes;
public class GetCourseTypesResultTests
{
    [Test, RecursiveMoqAutoData]

    public void GetCourseTypesResult_MapsCourseTypeDataCorrectly(CourseType courseType)
    {
        // Arrange
        var sut = new GetCourseTypesResult
        {
            CourseTypes = new List<CourseTypes> { courseType }
        };
        ;
        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(sut.CourseTypes.FirstOrDefault().Id, Is.EqualTo(courseType.Id));
            Assert.That(sut.CourseTypes.FirstOrDefault().Name, Is.EqualTo(courseType.Name));
            Assert.That(sut.CourseTypes.FirstOrDefault().LearningType, Is.EqualTo(courseType.LearningType));
        });
    }
}