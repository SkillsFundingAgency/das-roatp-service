using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisations;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class OrganisationsController(IMediator _mediator, ILogger<OrganisationsController> _logger) : ControllerBase
{
    [HttpGet]
    [Route("{ukprn:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrganisationQueryResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisationByUkprn([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Organisations-GetOrganisationByUkprn for UKPRN {Ukprn}", ukprn);
        GetOrganisationQuery query = new(ukprn);
        GetOrganisationQueryResult result = await _mediator.Send(query, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrganisationQueryResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisations(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Organisations-GetOrganisations");
        GetOrganisationsQueryResult result = await _mediator.Send(new GetOrganisationsQuery(), cancellationToken);
        return Ok(result);
    }
}
