using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationTypes;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("organisation-types")]
[Tags("LookupData")]
[ApiController]
public class OrganisationTypesController(IMediator _mediator, ILogger<OrganisationTypesController> _logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrganisationTypesQueryResult))]

    public async Task<IActionResult> GetOrganisationTypes(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get organisation types");
        GetOrganisationTypesQueryResult queryResult = await _mediator.Send(new GetOrganisationTypesQuery(), cancellationToken);
        return Ok(queryResult);
    }
}