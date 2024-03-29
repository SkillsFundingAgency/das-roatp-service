﻿using System;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Types;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain;

    public interface IAuditLogService
    {
        Task<AuditData> BuildListOfFieldsChanged(Organisation originalOrganisation, Organisation updatedOrganisation);
        AuditData CreateAuditLogEntry(Guid organisationId, string updatedBy, string fieldName,
            string oldValue, string newValue);
        AuditData CreateAuditData(Guid organisationId, string updatedBy);
        void AddAuditEntry(AuditData auditData, string fieldChanged, string previousValue, string newValue);
        AuditData AuditFinancialTrackRecord(Guid organisationId, string updatedBy, bool newFinancialTrackRecord);
        AuditData AuditParentCompanyGuarantee(Guid organisationId, string updatedBy, bool newParentCompanyGuarantee);
        AuditData AuditLegalName(Guid organisationId, string updatedBy, string newLegalName);
        AuditData AuditTradingName(Guid organisationId, string updatedBy, string newTradingName);
        AuditData AuditUkprn(Guid organisationId, string updatedBy, long newUkprn);
        AuditData AuditCompanyNumber(Guid organisationId, string updatedBy, string companyNumber);
        AuditData AuditOrganisationType(Guid organisationId, string updatedBy, int newOrganisationTypeId);
        AuditData AuditOrganisationStatus(Guid organisationId, string updatedBy, int newOrganisationStatusId,
            int? newRemovedReasonId);
        AuditData AuditProviderType(Guid organisationId, string updatedBy, int newProviderTypeId, int newOrganisationTypeId);
        AuditData AuditCharityNumber(Guid organisationId, string updatedBy, string charityNumber);
        AuditData AuditApplicationDeterminedDate(Guid organisationId, string updatedBy, DateTime applicationDeterminedDate);
        AuditData AuditOrganisation(UpdateOrganisationCommand command);
    }
}
