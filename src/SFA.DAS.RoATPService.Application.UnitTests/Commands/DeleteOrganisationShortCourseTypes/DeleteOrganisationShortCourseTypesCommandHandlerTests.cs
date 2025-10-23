using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.DeleteOrganisationShortCourseTypes;
public class DeleteOrganisationShortCourseTypesCommandHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_ShortCourseTypeFound_InvokesDeleteOrganisationShortCourseTypes(
        [Frozen] Mock<IOrganisationCourseTypesRepository> organisationCourseTypesRepositoryMock,
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        DeleteOrganisationShortCourseTypesCommand command)
    {
        // Arrange
        Organisation organisation = new()
        {
            OrganisationCourseTypes = new List<OrganisationCourseType>
                {  new OrganisationCourseType { CourseType = new CourseType { LearningType = LearningType.ShortCourse }}}
        };
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(organisation);
        DeleteOrganisationShortCourseTypesCommandHandler sut = new(organisationsRepositoryMock.Object, organisationCourseTypesRepositoryMock.Object);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        organisationCourseTypesRepositoryMock.Verify(o => o.DeleteOrganisationShortCourseTypes(organisation.Id, command.RequestingUserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_ShortCourseTypeNotFound_DoesNotInvokesDeleteOrganisationShortCourseTypes(
        [Frozen] Mock<IOrganisationCourseTypesRepository> organisationCourseTypesRepositoryMock,
        [Frozen] Mock<IOrganisationsRepository> organisationsRepositoryMock,
        DeleteOrganisationShortCourseTypesCommand command)
    {
        // Arrange
        Organisation organisation = new()
        {
            OrganisationCourseTypes = new List<OrganisationCourseType>
                {  new OrganisationCourseType { CourseType = new CourseType { LearningType = LearningType.Standard }}}
        };
        organisationsRepositoryMock.Setup(o => o.GetOrganisationByUkprn(command.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(organisation);
        DeleteOrganisationShortCourseTypesCommandHandler sut = new(organisationsRepositoryMock.Object, organisationCourseTypesRepositoryMock.Object);

        // Act
        await sut.Handle(command, CancellationToken.None);

        // Assert
        organisationCourseTypesRepositoryMock.Verify(o => o.DeleteOrganisationShortCourseTypes(organisation.Id, command.RequestingUserId, It.IsAny<CancellationToken>()), Times.Never);
    }
}