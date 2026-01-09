using System.Threading.Tasks;
using SFA.DAS.RoATPService.Ukrlp.Client.Models;

namespace SFA.DAS.RoATPService.Ukrlp.Client.Interfaces;

public interface IUkrlpApiClient
{
    Task<UkprnLookupResponse> GetTrainingProviderByUkprn(long ukprn);
}
