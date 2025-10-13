using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetAllCourseTypes;
public class CourseTypeSummaryTests
{
    [Test, RecursiveMoqAutoData]

    public void CourseTypeSummary_ImplicitOperator_MapsCourseTypeCorrectly(CourseType courseType)
    {
        // Act
        CourseTypeSummary result = courseType;

        // Assert
        result.Id.Should().Be(courseType.Id);
        result.Name.Should().Be(courseType.Name);
        result.LearningType.Should().Be(courseType.LearningType);
    }
}