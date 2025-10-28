using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Commands.UpdateOrganisationCourseTypes;
public class UpdateOrganisationCourseTypesCommandHandlerTests
{
    [Test, AutoData]
    public async Task Handle_UkprnNotFound_ReturnsFailedResponse(
        UpdateOrganisationCourseTypesCommand command)
    {
        Mock<IOrganisationCourseTypesRepository> orgCrsTypeRepoMock = new();
        Mock<IOrganisationsRepository> organisationsRepoMock = new();
        organisationsRepoMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, default)).ReturnsAsync(() => null);
        UpdateOrganisationCourseTypesCommandHandler sut = new(organisationsRepoMock.Object, orgCrsTypeRepoMock.Object);

        ValidatedResponse<SuccessModel> actual = await sut.Handle(command, default);

        orgCrsTypeRepoMock.Verify(x => x.UpdateOrganisationCourseTypes(It.IsAny<Organisation>(), It.IsAny<IEnumerable<int>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        actual.Result.IsSuccess.Should().BeFalse();
    }

    [Test, RecursiveMoqAutoData]
    public async Task Handle_GetsOrganisationId_InvokesUpdateCourseTypes(
        UpdateOrganisationCourseTypesCommand command,
        Organisation organisation)
    {
        Mock<IOrganisationCourseTypesRepository> orgCrsTypeRepoMock = new();
        Mock<IOrganisationsRepository> organisationsRepoMock = new();
        organisationsRepoMock.Setup(x => x.GetOrganisationByUkprn(command.Ukprn, default)).ReturnsAsync(organisation);
        UpdateOrganisationCourseTypesCommandHandler sut = new(organisationsRepoMock.Object, orgCrsTypeRepoMock.Object);

        var actual = await sut.Handle(command, default);

        orgCrsTypeRepoMock.Verify(x => x.UpdateOrganisationCourseTypes(organisation, command.CourseTypeIds, command.RequestingUserId, default), Times.Once);
        actual.Result.IsSuccess.Should().BeTrue();
    }
}
