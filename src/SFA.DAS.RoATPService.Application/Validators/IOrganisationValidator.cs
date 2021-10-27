﻿using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Application.Commands;
using SFA.DAS.RoATPService.Application.Types;

namespace SFA.DAS.RoATPService.Application.Validators
{
    using SFA.DAS.RoATPService.Domain;
    using System;
    using System.Threading.Tasks;

    public interface IOrganisationValidator
    {
        bool IsValidOrganisationId(Guid organisationId);
        bool IsValidUpdateOrganisation(UpdateOrganisationCommand command);
        bool IsValidProviderType(ProviderType providerType);
     
        bool IsValidUKPRN(long ukPrn);
        bool IsValidLegalName(string legalName);
        bool IsValidTradingName(string tradingName);
        bool IsValidStatusDate(DateTime statusDate);

        bool IsValidApplicationDeterminedDate(DateTime? applicationDeterminedDate);
        bool IsValidProviderTypeId(int providerTypeId);

        bool IsValidStatus(OrganisationStatus status);
        bool IsValidStatusId(int statusId);
        bool IsValidCompanyNumber(string companyNumber);
        bool IsValidCharityNumber(string charityNumber);
        bool IsValidOrganisationType(OrganisationType organisationType);
        bool IsValidOrganisationTypeId(int organisationTypeId);
        bool IsValidOrganisationTypeIdForOrganisation(int organisationTypeId, Guid organisationId);
        bool IsValidOrganisationStatusIdForOrganisation(int organisationStatusId, Guid organisationId);
        Task<bool> IsValidOrganisationTypeIdForProvider(int organisationTypeId, int providerTypeId);
        DuplicateCheckResponse DuplicateUkprnInAnotherOrganisation(long ukprn, Guid organisationId);
        DuplicateCheckResponse DuplicateCompanyNumberInAnotherOrganisation(string companyNumber, Guid organisationId);
        DuplicateCheckResponse DuplicateCharityNumberInAnotherOrganisation(string charityNumber, Guid organisationId);
        ValidationErrorMessage ValidateOrganisation(UpdateOrganisationCommand command);


    }
}
