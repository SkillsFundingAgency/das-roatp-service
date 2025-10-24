using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisationStatusHistory;
public class GetOrganisationStatusHistoryQueryHandlerTests
{
    [Test, RecursiveMoqAutoData]
    public async Task Handle_ReturnsExpectedResult(
        [Frozen] Mock<IOrganisationStatusEventsRepository> repoMock,
        GetOrganisationStatusHistoryQueryHandler sut,
        GetOrganisationStatusHistoryQuery query,
        List<OrganisationStatusEvent> statusEvents,
        CancellationToken cancellationToken)
    {
        repoMock.Setup(List => List.GetOrganisationStatusHistory(query.Ukprn, cancellationToken))
            .ReturnsAsync(statusEvents);

        var result = await sut.Handle(query, cancellationToken);

        Assert.AreEqual(statusEvents.Count, result.StatusHistory.Count());
        statusEvents.TrueForAll(e => result.StatusHistory.Any(r =>
            r.Status == e.OrganisationStatus &&
            r.AppliedDate == e.CreatedOn));
    }
}
