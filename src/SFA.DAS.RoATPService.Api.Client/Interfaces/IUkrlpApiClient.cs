﻿using System.Collections.Generic;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Api.Client.Interfaces
{
    public interface IUkrlpApiClient
    {
        Task<UkprnLookupResponse> GetTrainingProviderByUkprn(long ukprn);
        Task<UkprnLookupResponse> GetListOfTrainingProviders(List<long> ukprns);
    }
}
