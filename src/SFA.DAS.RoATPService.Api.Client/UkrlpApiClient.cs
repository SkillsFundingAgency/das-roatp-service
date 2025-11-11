using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Client.Interfaces;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;
using SFA.DAS.RoATPService.Application.Api.Configuration;

namespace SFA.DAS.RoATPService.Api.Client;

public class UkrlpApiClient : IUkrlpApiClient
{
    private readonly ILogger<UkrlpApiClient> _logger;

    private readonly UkrlpApiAuthentication _config;

    private readonly HttpClient _httpClient;

    private readonly IUkrlpSoapSerializer _serializer;

    public UkrlpApiClient(ILogger<UkrlpApiClient> logger, UkrlpApiAuthentication config, HttpClient httpClient, IUkrlpSoapSerializer serializer)
    {
        _logger = logger;
        _config = config;
        _httpClient = httpClient;
        _serializer = serializer;
    }

    public async Task<UkprnLookupResponse> GetTrainingProviderByUkprn(long ukprn)
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

    private async Task<UkprnLookupResponse> GetUkprnLookupResponse(string request)
    {
        _logger.LogInformation("Calling UKRLP API with request: {Request}", request);
        var requestMessage =
            new HttpRequestMessage(HttpMethod.Post, _config.ApiBaseAddress)
            {
                Content = new StringContent(request, Encoding.UTF8, "text/xml")
            };

        var responseMessage = await _httpClient.SendAsync(requestMessage);

        if (!responseMessage.IsSuccessStatusCode)
        {
            var failureResponse = new UkprnLookupResponse
            {
                Success = false,
                Results = new List<ProviderDetails>()
            };
            return await Task.FromResult(failureResponse);
        }

        var soapXml = await responseMessage.Content.ReadAsStringAsync();
        var matchingProviderRecords = _serializer.DeserialiseMatchingProviderRecordsResponse(soapXml);

        if (matchingProviderRecords != null)
        {
            var result = matchingProviderRecords.Select(Mapper.Map<ProviderDetails>).ToList();

            var resultsFound = new UkprnLookupResponse
            {
                Success = true,
                Results = result
            };
            return await Task.FromResult(resultsFound);
        }
        else
        {
            var noResultsFound = new UkprnLookupResponse
            {
                Success = true,
                Results = []
            };
            return await Task.FromResult(noResultsFound);
        }
    }
}
