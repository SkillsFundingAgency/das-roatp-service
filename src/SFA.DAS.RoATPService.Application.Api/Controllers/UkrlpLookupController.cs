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

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("")]
[Tags("Ukrlp-Lookup")]
public class UkrlpLookupController(IUkrlpService _ukrlpService) : ControllerBase
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
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("organisations/{ukprn}/ukrlp-data")]
    public async Task<IActionResult> UkrlpLookup(int ukprn, CancellationToken cancellationToken)
    {
        var request = new UkrlpQuery(null, [ukprn]);

        UkrlpQueryResult response = await _ukrlpService.GetProviderDataAsync(request, cancellationToken);

        return Ok(new UkrlpLookupModel(response.Success, response.Providers.Select(p => (ProviderDetails)p)));
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
}
