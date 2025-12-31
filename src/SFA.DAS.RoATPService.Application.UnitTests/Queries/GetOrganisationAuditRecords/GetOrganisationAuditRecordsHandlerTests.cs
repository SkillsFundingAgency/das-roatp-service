using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationAuditRecords;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisationAuditRecords;

public class GetOrganisationAuditRecordsHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handler_WhenCalled_ReturnsRecords(
        List<OrganisationAudit> expected,
        [Frozen] Mock<IAuditsRepository> auditRepositoryMock,
        GetOrganisationAuditRecordsQueryHandler sut,
        CancellationToken cancellationToken)
    {
        auditRepositoryMock.Setup(r => r.GetOrganisationAuditRecords(cancellationToken)).ReturnsAsync(expected);

        var actual = await sut.Handle(new GetOrganisationAuditRecordsQuery(), cancellationToken);

        actual.AuditRecords.Should().BeEquivalentTo(expected.Select(r => (OrganisationAuditRecord)r));
    }
}
