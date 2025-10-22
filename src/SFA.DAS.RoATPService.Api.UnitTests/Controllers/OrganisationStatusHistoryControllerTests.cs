using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Api.Controllers;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Api.UnitTests.Controllers;

public class OrganisationStatusHistoryControllerTests
{
    [Test, MoqAutoData]
    public async Task GetAll_ReturnsOkResult(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] OrganisationStatusHistoryController sut,
        int ukprn,
        GetOrganisationStatusHistoryQueryResult queryResult,
        CancellationToken cancellationToken)
    {
        mediatorMock
            .Setup(m => m.Send(It.Is<GetOrganisationStatusHistoryQuery>(q => q.Ukprn == ukprn), cancellationToken))
            .ReturnsAsync(queryResult);

        var result = await sut.GetAll(ukprn, cancellationToken);

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        result.As<OkObjectResult>().Value.Should().Be(queryResult);
    }
}
