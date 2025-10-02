using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationAllowedShortCourses;
public class UpdateOrganisationAllowedShortCoursesCommandHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_GetsOrganisationId_InvokesUpdateCourseTypes(
        UpdateOrganisationAllowedShortCoursesCommand command,
        Organisation organisation)
    {
        Mock<IOrganisationCourseTypesRepository> orgCrsTypeRepoMock = new();
        Mock<IOrganisationsRepository> organisationsRepoMock = new();
        organisationsRepoMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, default)).ReturnsAsync(organisation);
        UpdateOrganisationAllowedShortCoursesCommandHandler sut = new(organisationsRepoMock.Object, orgCrsTypeRepoMock.Object);

        await sut.Handle(command, default);

        orgCrsTypeRepoMock.Verify(x => x.UpdateOrganisationShortCourseTypes(organisation.Id, command.CourseTypeIds, command.RequestingUserId, default), Times.Once);
    }
}
