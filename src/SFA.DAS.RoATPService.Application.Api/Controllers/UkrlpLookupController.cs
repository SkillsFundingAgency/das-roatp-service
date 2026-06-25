using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Ukrlp.Client;
using SFA.DAS.RoATPService.Ukrlp.Client.SoapClient;
using SFA.DAS.RoATPService.Ukrlp.SoapClient;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("")]
[Tags("Ukrlp-Lookup")]
public class UkrlpLookupController(IUkrlpService _ukrlpService, IUkrlpSoapApiClient _ukrlpSoapApiClient) : ControllerBase
{
    /// <summary>
    /// This endpoint is consumed by Roatp Apply journies. Ideally we want to move towards using the GetProviders endpoint, 
    /// but this is a more direct way to get the data we need for the Apply journeys without having to change the code in Apply at this time. 
    /// </summary>
    /// <param name="ukprn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("This endpoint is being deprecated in favour of the GetProviders endpoint.")]
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UkrlpLookupModel))]
    [Route("organisations/{ukprn}/ukrlp-data")]
    public async Task<IActionResult> UkrlpLookup(int ukprn, CancellationToken cancellationToken)
    {
        var request = new UkrlpQuery(null, [ukprn]);

        UkrlpQueryResult response = await _ukrlpService.GetProviderDataAsync(request, cancellationToken);
        if (response.Providers.Any())
        {
            response.Providers.SelectMany(p => p.VerificationDetails).ToList().ForEach(c => c.VerificationAuthority = TransformValidationAuthority(c.VerificationAuthority));

            return Ok(new UkrlpLookupModel(response.Success, response.Providers.Select(p => (ProviderDetails)p)));
        }

        // This is a temporary fallback attempt to get provider details via soap api
        UkrlpLookupResponse soapResponse = await _ukrlpSoapApiClient.GetTrainingProviderByUkprn(ukprn);
        return Ok(new UkrlpLookupModel(soapResponse.Success, soapResponse.Results.Select(s => (ProviderDetails)s)));
    }

    [HttpGet]
    [Route("ukrlp/providers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UkrlpProvidersModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(IDictionary<string, string>))]
    public async Task<IActionResult> GetProviders([FromQuery] int[] ukprns, [FromQuery] DateTime? updatedSince, CancellationToken cancellationToken)
    {
        UkrlpQueryResult apiResponse = await _ukrlpService.GetProviderDataAsync(new UkrlpQuery(updatedSince, ukprns), cancellationToken);
        if (!apiResponse.Success) return StatusCode(StatusCodes.Status500InternalServerError, new Dictionary<string, string> { { "Error", "Failed to retrieve provider data from UKRLP service" } });
        return Ok(new UkrlpProvidersModel(apiResponse.Providers.Select(p => (ProviderModel)p)));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProviderModel))]
    [Route("ukrlp/providers/{ukprn}")]
    public async Task<IActionResult> GetProvider(int ukprn, CancellationToken cancellationToken)
    {
        var request = new UkrlpQuery(null, [ukprn]);

        UkrlpQueryResult response = await _ukrlpService.GetProviderDataAsync(request, cancellationToken);
        var provider = response.Providers.FirstOrDefault();
        if (provider != null)
        {
            ProviderModel result = provider;
            return Ok(result);
        }

        // This is a temporary fallback attempt to get provider details via soap api
        UkrlpLookupResponse soapResponse = await _ukrlpSoapApiClient.GetTrainingProviderByUkprn(ukprn);
        if (soapResponse.Success && soapResponse.Results.Count != 0)
        {
            ProviderModel result = soapResponse.Results.First();
            return Ok(result);
        }

        return NotFound();
    }

    private static string TransformValidationAuthority(string validationAuthority)
        => validationAuthority switch
        {
            "ISC" => "Independent Schools Council",
            "SCC" => "Scottish Executive Education Department",
            "CHARITY" => "Charity Commission",
            "URN" => "DfE(Schools Unique Reference Number)",
            "NII" => "Department of Education in Northern Ireland",
            "SFA" => "SFA Validated",
            "SI" => "Government Statute",
            "COMPANY" => "Companies House",
            "SOLE" => "Sole Trader or Non-limited Partnership",
            "DFES" => "DfE(LEA Code and Establishment Number)",
            _ => "Unknown"
        };
}
