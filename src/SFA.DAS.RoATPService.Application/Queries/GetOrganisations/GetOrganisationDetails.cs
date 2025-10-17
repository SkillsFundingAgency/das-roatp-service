using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;
using SFA.DAS.RoATPService.Domain.Entities;


namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisations;


public class GetOrganisationDetails
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
    public DateTime? ApplicationDeterminedDate { get; set; }
    public int? RemovedReasonId { get; set; }
    public string RemovedReason { get; set; }
    public IEnumerable<AllowedCourseType> AllowedCourseTypes { get; set; } = [];

    public static implicit operator GetOrganisationDetails(Organisation source) =>
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
            CompanyNumber = source.OrganisationData?.CompanyNumber,
            CharityNumber = source.OrganisationData?.CharityNumber,
            ApplicationDeterminedDate = source.OrganisationData?.ApplicationDeterminedDate,
            RemovedReasonId = source.OrganisationData?.RemovedReason?.Id,
            RemovedReason = source.OrganisationData?.RemovedReason?.Reason,
            AllowedCourseTypes = source.OrganisationCourseTypes.Select(c => new AllowedCourseType(c.CourseType.Id, c.CourseType.Name, c.CourseType.LearningType))
        };
}
