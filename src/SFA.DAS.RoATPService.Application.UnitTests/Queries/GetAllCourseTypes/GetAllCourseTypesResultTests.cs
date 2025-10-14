using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetAllCourseTypes;
public class GetAllCourseTypesResultTests
{
    [Test, RecursiveMoqAutoData]

    public void GetAllCourseTypesResult_MapsCourseTypeDataCorrectly(CourseType courseType)
    {
        // Arrange
        CourseTypeSummary courseTypeSummary = courseType;
        GetAllCourseTypesResult sut = new() { CourseTypes = [courseTypeSummary] };

        // Act
        var result = sut.CourseTypes.First();

        // Assert
        result.Id.Should().Be(courseType.Id);
        result.Name.Should().Be(courseType.Name);
        result.LearningType.Should().Be(courseType.LearningType);
    }
}