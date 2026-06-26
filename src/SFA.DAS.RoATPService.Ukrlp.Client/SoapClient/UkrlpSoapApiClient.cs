using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Ukrlp.Client.SoapClient;

namespace SFA.DAS.RoATPService.Ukrlp.SoapClient;

public interface IUkrlpSoapApiClient
{
    Task<UkrlpLookupResponse> GetTrainingProviderByUkprn(long ukprn);
}

[ExcludeFromCodeCoverage]
public class UkrlpSoapApiClient : IUkrlpSoapApiClient
{
    private readonly ILogger<UkrlpSoapApiClient> _logger;

    private readonly UkrlpSoapApiAuthentication _config;

    private readonly HttpClient _httpClient;

    private readonly IUkrlpSoapSerializer _serializer;

    public UkrlpSoapApiClient(ILogger<UkrlpSoapApiClient> logger, UkrlpSoapApiAuthentication config, HttpClient httpClient, IUkrlpSoapSerializer serializer)
    {
        _logger = logger;
        _config = config;
        _httpClient = httpClient;
        _serializer = serializer;
    }

    public async Task<UkrlpLookupResponse> GetTrainingProviderByUkprn(long ukprn)
    {
        // Due to a bug in .net core, we have to parse the SOAP XML from UKRLP by hand
        // If this ever gets fixed then look to remove this code and replace with 'Add connected service'
        // https://github.com/dotnet/wcf/issues/3228
        // Sep 2025 Update: I tried generating the client from the WSDL but it didn't serialise one class with different namespace, so I was not able to use the generated client.
        _logger.LogInformation("Calling UKRLP API to get details for UKPRN: {Ukprn}", ukprn);
        var request = _serializer.BuildUkrlpSoapRequest(ukprn, _config.StakeholderId,
            _config.QueryId);

        return await GetUkprnLookupResponse(request);
    }

    private async Task<UkrlpLookupResponse> GetUkprnLookupResponse(string request)
    {
        _logger.LogInformation("Calling UKRLP API with request: {Request}", request);
        var requestMessage =
            new HttpRequestMessage(HttpMethod.Post, _config.ApiBaseAddress)
            {
                Content = new StringContent(request, Encoding.UTF8, "text/xml")
            };

        var responseMessage = await _httpClient.SendAsync(requestMessage);

        if (!responseMessage.IsSuccessStatusCode) return new UkrlpLookupResponse(false, []);

        var soapXml = await responseMessage.Content.ReadAsStringAsync();
        List<MatchingProviderRecords> matchingProviderRecords = _serializer.DeserialiseMatchingProviderRecordsResponse(soapXml);

        return new UkrlpLookupResponse(true, matchingProviderRecords ?? []);
    }
}
