using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationAuditRecords;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers;

public class OrganisationAuditControllerTests
{
    [Test, MoqAutoData]
    public async Task GetFullAuditHistory_InvokesMediator_ReturnsOk(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationAuditController sut,
        GetOrganisationAuditRecordsQueryResult expected,
        CancellationToken cancellationToken)
    {
        mediatorMock.Setup(m => m.Send(It.IsAny<GetOrganisationAuditRecordsQuery>(), cancellationToken)).ReturnsAsync(expected);

        var result = await sut.GetFullAuditHistory(cancellationToken);

        mediatorMock.Verify(m => m.Send(It.IsAny<GetOrganisationAuditRecordsQuery>(), cancellationToken), Times.Once);
        result.As<OkObjectResult>().Value.Should().Be(expected);
    }
}
