using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;
using SFA.DAS.RoATPService.Ukrlp.Models;

namespace SFA.DAS.RoATPService.Ukrlp;
public class UkrlpApiClient(ProviderQueryPortTypeClient _ukrlpApiClient, UkrlpApiConfiguration _config) : IUkrlpApiClient
{
    public Task<UkprnLookupResponse> GetListOfTrainingProviders(List<long> ukprns)
    {
        return GetUkprnLookupResponse(ukprns.Select(u => u.ToString()).ToArray());
    }

    public Task<UkprnLookupResponse> GetTrainingProviderByUkprn(long ukprn)
    {
        return GetUkprnLookupResponse(new[] { ukprn.ToString() });
    }

    private async Task<UkprnLookupResponse> GetUkprnLookupResponse(string[] ukprns)
    {
        ProviderQueryStructure query = new()
        {
            QueryId = _config.QueryId,
            SelectionCriteria = new()
            {
                UnitedKingdomProviderReferenceNumberList = ukprns,
                CriteriaCondition = QueryCriteriaConditionType.OR,
                CriteriaConditionSpecified = true,
                StakeholderId = _config.StakeholderId,
                ApprovedProvidersOnly = YesNoType.No,
                ApprovedProvidersOnlySpecified = true,
                ProviderStatus = "A"
            }
        };

        response response = await _ukrlpApiClient.retrieveAllProvidersAsync(new ProviderQueryParam(query));
        ProviderQueryResponse result = response.ProviderQueryResponse;

        if (result.MatchingProviderRecords == null || result.MatchingProviderRecords.Length == 0)
        {
            return new UkprnLookupResponse
            {
                Success = true,
                Results = []
            };
        }

        return new UkprnLookupResponse
        {
            Success = true,
            Results = result.MatchingProviderRecords.Select(r => (ProviderDetails)r)
        };
    }
}
