using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using SFA.DAS.RoATPService.Application.Queries.GetAllProviderTypes;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("provider-types")]
[Tags("LookupData")]
[ApiController]
public class ProviderTypesController(IMediator _mediator, ILogger<ProviderTypesController> _logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCourseTypesResult))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing get all provider types");
        GetAllProviderTypesQueryResult result = await _mediator.Send(new GetAllProviderTypesQuery(), cancellationToken);
        return Ok(result);
    }
}
