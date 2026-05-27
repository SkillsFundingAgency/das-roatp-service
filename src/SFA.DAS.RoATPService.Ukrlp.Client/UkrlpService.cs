using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Ukrlp.Client;

public interface IUkrlpService
{
    Task<UkrlpResponse> GetProviderDataAsync(UkrlpRequest request, CancellationToken cancellationToken = default);
}

public class UkrlpService(HttpClient _httpClient) : IUkrlpService
{
    public async Task<UkrlpResponse> GetProviderDataAsync(UkrlpRequest request, CancellationToken cancellationToken = default)
    {
        var queryParamsList = new List<string>();
        queryParamsList.AddRange(request.Ukprns.Select(ukprn => $"ukprns={ukprn}"));
        if (request.UpdatedSinceDate.HasValue)
        {
            queryParamsList.Add($"updatedSince={request.UpdatedSinceDate.Value.ToString("yyyy-MM-dd")}");
        }
        var queryParams = string.Join("&", queryParamsList);
        var response = await _httpClient.GetAsync($"api/providers?{queryParams}", cancellationToken);
        if (!response.IsSuccessStatusCode) return new UkrlpResponse(false, []);

        List<MatchingProviders> matchingProviders = await ParseNdjsonAsync(response, cancellationToken);
        return new UkrlpResponse(true, matchingProviders.SelectMany(c => c.MatchingProviderRecords));
    }

    private static async Task<List<MatchingProviders>> ParseNdjsonAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var results = new List<MatchingProviders>();
        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        string line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var obj = JsonSerializer.Deserialize<MatchingProviders>(line);
            if (obj != null)
                results.Add(obj);
        }

        return results;
    }
}
