using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.RoATPService.Application.UnitTests.Queries.GetOrganisation;

public class GetOrganisationQueryResultTests
{
    [Test, RecursiveMoqAutoData]
    public void ConvertsFromOrganisationEntity(Organisation organisation)
    {
        GetOrganisationQueryResult sut = organisation;
        Assert.Multiple(() =>
        {
            Assert.That(sut.OrganisationId, Is.EqualTo(organisation.Id));
            Assert.That(sut.Ukprn, Is.EqualTo(organisation.Ukprn));
            Assert.That(sut.LegalName, Is.EqualTo(organisation.LegalName));
            Assert.That(sut.TradingName, Is.EqualTo(organisation.TradingName));
            Assert.That(sut.CompanyNumber, Is.EqualTo(organisation.OrganisationData.CompanyNumber));
            Assert.That(sut.CharityNumber, Is.EqualTo(organisation.OrganisationData.CharityNumber));
            Assert.That(sut.ProviderType, Is.EqualTo(organisation.ProviderType));
            Assert.That(sut.OrganisationType, Is.EqualTo(organisation.OrganisationType));
            Assert.That(sut.LastUpdatedDate, Is.EqualTo(organisation.UpdatedAt));
            Assert.That(sut.ApplicationDeterminedDate, Is.EqualTo(organisation.OrganisationData.ApplicationDeterminedDate));
            Assert.That(sut.Status, Is.EqualTo(organisation.Status));
            Assert.That(sut.RemovedReason, Is.EqualTo(organisation.OrganisationData?.RemovedReason?.Reason));
            Assert.That(sut.AllowedCourseTypes.Count(), Is.EqualTo(organisation.OrganisationCourseTypes.Count));
        });
    }

    [Test, RecursiveMoqAutoData]
    public void ConvertsAllowedCoursesTypeFromCourseTypeEntity(Organisation organisation)
    {
        GetOrganisationQueryResult sut = organisation;
        sut.AllowedCourseTypes.Should().BeEquivalentTo(organisation.OrganisationCourseTypes, options => options.ExcludingMissingMembers());
    }
}
