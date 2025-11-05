using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Queries.GetRemovedReasons;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("removed-reasons")]
[Tags("LookupData")]
[ApiController]
public class RemovedReasonsController(IMediator _mediator, ILogger<RemovedReasonsController> _logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRemovedReasonsQueryResult))]
    public async Task<IActionResult> GetAllRemovedReasons(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to get all removed reasons.");
        GetRemovedReasonsQueryResult result = await _mediator.Send(new GetRemovedReasonsQuery(), cancellationToken);
        return Ok(result);
    }
}