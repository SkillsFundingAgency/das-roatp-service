using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SFA.DAS.RoATPService.Api.Client.Interfaces;
using SFA.DAS.RoATPService.Api.Client.Models.Ukrlp;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("organisations")]
[Tags("Ukrlp-Lookup")]
public class UkrlpLookupController : ControllerBase
{
    private readonly ILogger<UkrlpLookupController> _logger;

    private readonly IUkrlpApiClient _apiClient;

    private readonly AsyncRetryPolicy _retryPolicy;

    public UkrlpLookupController(ILogger<UkrlpLookupController> logger, IUkrlpApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
        _retryPolicy = GetRetryPolicy();
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UkprnLookupResponse))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("{ukprn}/ukrlp-data")]
    public async Task<IActionResult> UkrlpLookup(int ukprn)
    {
        UkprnLookupResponse providerData;

        try
        {
            providerData = await _retryPolicy.ExecuteAsync(context => _apiClient.GetTrainingProviderByUkprn(ukprn), new Context());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to retrieve results from UKRLP");
            providerData = new UkprnLookupResponse
            {
                Success = false,
                Results = new List<ProviderDetails>()
            };
        }
        return Ok(providerData);
    }

    private AsyncRetryPolicy GetRetryPolicy()
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(4)
            }, (exception, timeSpan, retryCount, context) =>
            {
                _logger.LogWarning("Error retrieving response from UKRLP. Reason: {ErrorMessage}. Retrying in {Seconds} secs...attempt: {RetryCount}", exception.Message, timeSpan.Seconds, retryCount);
            });
    }
}
