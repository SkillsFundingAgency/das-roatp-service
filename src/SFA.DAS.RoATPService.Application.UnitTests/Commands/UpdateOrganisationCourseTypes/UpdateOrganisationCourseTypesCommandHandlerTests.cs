using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationCourseTypes;
public class UpdateOrganisationCourseTypesCommandHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_GetsOrganisationId_InvokesUpdateCourseTypes(
        UpdateOrganisationCourseTypesCommand command,
        Organisation organisation)
    {
        Mock<IOrganisationCourseTypesRepository> orgCrsTypeRepoMock = new();
        Mock<IOrganisationsRepository> organisationsRepoMock = new();
        organisationsRepoMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, default)).ReturnsAsync(organisation);
        UpdateOrganisationCourseTypesCommandHandler sut = new(organisationsRepoMock.Object, orgCrsTypeRepoMock.Object);

        await sut.Handle(command, default);

        orgCrsTypeRepoMock.Verify(x => x.UpdateOrganisationCourseTypes(organisation, command.CourseTypeIds, command.RequestingUserId, default), Times.Once);
    }
}
