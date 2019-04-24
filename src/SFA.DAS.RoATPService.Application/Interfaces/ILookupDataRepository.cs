﻿using System;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    using Domain;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ILookupDataRepository
    {
        Task<IEnumerable<ProviderType>> GetProviderTypes();
        Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId);

        Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId);
        Task<OrganisationType> GetOrganisationType(int organisationTypeId);
        Task<IEnumerable<RemovedReason>> GetRemovedReasons();

        Task<OrganisationStatus> GetOrganisationStatus(int statusId);

        Task<bool> IsOrganisationTypeValidForOrganisation(int organisationTypeId, Guid organisationId);
        Task<bool> IsOrganisationStatusValidForOrganisation(int organisationStatusId, Guid organisationId);
    }
}
