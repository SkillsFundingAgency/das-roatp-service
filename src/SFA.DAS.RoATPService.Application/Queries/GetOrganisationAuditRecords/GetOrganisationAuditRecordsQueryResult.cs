using System;
using System.Collections.Generic;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Application.Queries.GetOrganisationAuditRecords;

public record GetOrganisationAuditRecordsQueryResult(IEnumerable<OrganisationAuditRecord> AuditRecords);

public class OrganisationAuditRecord
{
    public int Ukprn { get; set; }
    public string LegalName { get; set; }
    public string FieldChanged { get; set; }
    public string PreviousValue { get; set; }
    public string NewValue { get; set; }
    public DateTime? PreviousStatusDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }

    public static implicit operator OrganisationAuditRecord(OrganisationAudit v)
        => new OrganisationAuditRecord
        {
            Ukprn = v.Ukprn,
            LegalName = v.LegalName,
            FieldChanged = v.FieldChanged,
            PreviousValue = v.PreviousValue,
            NewValue = v.NewValue,
            PreviousStatusDate = v.PreviousStatusDate,
            UpdatedAt = v.UpdatedAt,
            UpdatedBy = v.UpdatedBy
        };
}
