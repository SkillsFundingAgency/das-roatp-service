using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;

namespace SFA.DAS.RoATPService.Ukrlp;

public interface IUkrlpApiClient
{
    Task<UkprnLookupResponse> GetTrainingProviderByUkprn(long ukprn);
    Task<UkprnLookupResponse> GetListOfTrainingProviders(List<long> ukprns);
}
