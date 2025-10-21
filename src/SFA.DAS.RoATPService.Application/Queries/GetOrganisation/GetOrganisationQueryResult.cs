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
    public DateTime? StartDate { get; set; }
    public IEnumerable<AllowedCourseType> AllowedCourseTypes { get; set; } = [];

    public static implicit operator GetOrganisationQueryResult(Organisation source) =>
        new()
        {
            OrganisationId = source.Id,
            Ukprn = source.Ukprn,
            LegalName = source.LegalName,
            TradingName = source.TradingName,
            ProviderType = source.ProviderType,
            Status = source.Status,
            OrganisationTypeId = source.OrganisationType.Id,
            OrganisationType = source.OrganisationType.Type,
            CompanyNumber = source.CompanyNumber,
            CharityNumber = source.CharityNumber,
            ApplicationDeterminedDate = source.ApplicationDeterminedDate,
            RemovedReasonId = source.RemovedReasonId,
            RemovedReason = source.RemovedReason?.Reason,
            AllowedCourseTypes = source.OrganisationCourseTypes.Select(c => new AllowedCourseType(c.CourseType.Id, c.CourseType.Name, c.CourseType.LearningType)),
            StartDate = source.StartDate
        };
}

public record AllowedCourseType(int CourseTypeId, string CourseTypeName, LearningType LearningType);