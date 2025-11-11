using System.Threading.Tasks;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;

namespace SFA.DAS.RoATPService.Api.Client.Interfaces;

public interface IUkrlpApiClient
{
    Task<UkprnLookupResponse> GetTrainingProviderByUkprn(long ukprn);
}
