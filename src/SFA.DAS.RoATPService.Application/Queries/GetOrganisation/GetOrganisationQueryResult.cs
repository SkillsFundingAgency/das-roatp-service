using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisation;

public class GetOrganisationQueryResult
{
    public Guid OrganisationId { get; set; }
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string TradingName { get; set; }
    public string CompanyNumber { get; set; }
    public string CharityNumber { get; set; }
    public ProviderType ProviderType { get; set; }
    public int OrganisationTypeId { get; set; }
    public string OrganisationType { get; set; }
    public OrganisationStatus Status { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
    public DateTime? ApplicationDeterminedDate { get; set; }
    public int? RemovedReasonId { get; set; }
    public string RemovedReason { get; set; }
    public DateTime? RemovedDate { get; set; }
    public IEnumerable<AllowedCourseType> AllowedCourseTypes { get; set; } = [];

    public static implicit operator GetOrganisationQueryResult(Organisation source) =>
        new()
        {
            OrganisationId = source.Id,
            Ukprn = source.Ukprn,
            LegalName = source.LegalName,
            TradingName = source.TradingName,
            CompanyNumber = source.OrganisationData.CompanyNumber,
            CharityNumber = source.OrganisationData.CharityNumber,
            ProviderType = source.ProviderType,
            OrganisationTypeId = source.OrganisationType.Id,
            OrganisationType = source.OrganisationType.Type,
            LastUpdatedDate = source.UpdatedAt,
            ApplicationDeterminedDate = source.OrganisationData.ApplicationDeterminedDate,
            Status = source.Status,
            RemovedReasonId = source.OrganisationData?.RemovedReason?.Id,
            RemovedReason = source.OrganisationData?.RemovedReason?.Reason,
            AllowedCourseTypes = source.OrganisationCourseTypes.Select(c => new AllowedCourseType(c.CourseType.Id, c.CourseType.Name, c.CourseType.LearningType))
        };
}

public record AllowedCourseType(int CourseTypeId, string CourseTypeName, LearningType LearningType);