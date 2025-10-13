using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetCourseTypes;
public class GetAllCourseTypesResultTests
{
    [Test, RecursiveMoqAutoData]

    public void GetAllCourseTypesResult_MapsCourseTypeDataCorrectly(CourseType courseType)
    {
        // Arrange
        CourseTypeSummary sut = courseType;

        // Act & Assert
        sut.Id.Should().Be(courseType.Id);
        sut.Name.Should().Be(courseType.Name);
        sut.LearningType.Should().Be(courseType.LearningType);
    }
}