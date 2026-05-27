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
[Route("organisations")]
[Tags("Ukrlp-Lookup")]
public class UkrlpLookupController(IUkrlpService _ukrlpService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UkrlpLookupModel))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("{ukprn}/ukrlp-data")]
    public async Task<IActionResult> UkrlpLookup(int ukprn, CancellationToken cancellationToken)
    {
        var request = new UkrlpRequest(null, [ukprn]);

        UkrlpResponse response = await _ukrlpService.GetProviderDataAsync(request, cancellationToken);

        return Ok(new UkrlpLookupModel(response.Success, response.Providers.Select(p => (ProviderDetails)p)));
    }
}
