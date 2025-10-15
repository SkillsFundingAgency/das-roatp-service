using System.Linq;
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
            Assert.That(sut.CompanyNumber, Is.EqualTo(organisation.CompanyNumber));
            Assert.That(sut.CharityNumber, Is.EqualTo(organisation.CharityNumber));
            Assert.That(sut.ProviderType, Is.EqualTo(organisation.ProviderType));
            Assert.That(sut.OrganisationTypeId, Is.EqualTo(organisation.OrganisationType.Id));
            Assert.That(sut.OrganisationType, Is.EqualTo(organisation.OrganisationType.Type));
            Assert.That(sut.ApplicationDeterminedDate, Is.EqualTo(organisation.ApplicationDeterminedDate));
            Assert.That(sut.Status, Is.EqualTo(organisation.Status));
            Assert.That(sut.RemovedReasonId, Is.EqualTo(organisation.RemovedReasonId));
            Assert.That(sut.RemovedReason, Is.EqualTo(organisation.RemovedReason.Reason));
            Assert.That(sut.AllowedCourseTypes.Count(), Is.EqualTo(organisation.OrganisationCourseTypes.Count));
        });
    }

    [Test, RecursiveMoqAutoData]
    public void ImplicitOperator_MapsAllowedCourseTypesCorrectly(Organisation organisation)
    {
        // Arrange
        var courseType1 = new CourseType { Id = 1, Name = "Apprenticeship", LearningType = LearningType.Standard };
        var courseType2 = new CourseType { Id = 2, Name = "Unit", LearningType = LearningType.ShortCourse };

        var orgCourseType1 = new OrganisationCourseType { CourseType = courseType1 };
        var orgCourseType2 = new OrganisationCourseType { CourseType = courseType2 };

        organisation.OrganisationCourseTypes = [orgCourseType1, orgCourseType2];

        // Act
        GetOrganisationQueryResult result = organisation;

        // Assert
        var allowedCourseTypes = result.AllowedCourseTypes.ToList();
        Assert.That(allowedCourseTypes.Count, Is.EqualTo(2));

        Assert.That(allowedCourseTypes[0].CourseTypeId, Is.EqualTo(courseType1.Id));
        Assert.That(allowedCourseTypes[0].CourseTypeName, Is.EqualTo(courseType1.Name));
        Assert.That(allowedCourseTypes[0].LearningType, Is.EqualTo(courseType1.LearningType));

        Assert.That(allowedCourseTypes[1].CourseTypeId, Is.EqualTo(courseType2.Id));
        Assert.That(allowedCourseTypes[1].CourseTypeName, Is.EqualTo(courseType2.Name));
        Assert.That(allowedCourseTypes[1].LearningType, Is.EqualTo(courseType2.LearningType));
    }
}
