using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetAllCourseTypes;
public class GetAllCourseTypesQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]

    public async Task Handle_GetAllCourseTypes_ReturnsCourseTypes(
        [Frozen] Mock<ICourseTypesRepository> mockCourseTypesRepository,
        GetAllCourseTypesQueryHandler sut,
        GetAllCourseTypesQuery query,
        List<CourseType> expectedCourseTypes
        )
    {
        //Arrange
        mockCourseTypesRepository.Setup(x => x.GetAllCourseTypes(CancellationToken.None)).ReturnsAsync(expectedCourseTypes);

        //Act
        var response = await sut.Handle(query, CancellationToken.None);

        //Assert
        response.CourseTypes.Should().BeEquivalentTo(expectedCourseTypes, options => options.ExcludingMissingMembers()
        .Excluding(i => i.IsActive)
        .Excluding(o => o.OrganisationCourseTypes));
        mockCourseTypesRepository.Verify(x => x.GetAllCourseTypes(CancellationToken.None), Times.Once);
    }

    [Test, MoqAutoData]

    public async Task Handle_GetAllCourseTypes_ReturnsEmpty(
        [Frozen] Mock<ICourseTypesRepository> mockCourseTypesRepository,
        GetAllCourseTypesQueryHandler sut,
        GetAllCourseTypesQuery query
        )
    {
        //Arrange
        var expectedCourseTypes = new List<CourseType>();
        var expectedResponse = new GetAllCourseTypesResult();
        mockCourseTypesRepository.Setup(x => x.GetAllCourseTypes(CancellationToken.None)).ReturnsAsync(expectedCourseTypes);

        //Act
        var response = await sut.Handle(query, CancellationToken.None);

        //Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
}
