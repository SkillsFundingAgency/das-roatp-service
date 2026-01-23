using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationsStatusEvents;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("organisations")]
[Tags("Organisations Change Events")]
public class OrganisationStatusEventsController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [Route("{ukprn}/status-history")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrganisationStatusHistoryQueryResult))]
    public async Task<IActionResult> GetStatusEventsForUkprn([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        GetOrganisationStatusHistoryQuery query = new(ukprn);
        GetOrganisationStatusHistoryQueryResult result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// This endpoint is used by external FCS systems to create provider agreements etc.
    /// This was reintroduced after deleting as their PROD systems are dependent on this endpoint.
    /// </summary>
    /// <param name="sinceEventId"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("status-events")]
    [Route("/api/v1/organisation/engagements")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrganisationStatusEvent>))]
    public async Task<IActionResult> GetAllStatusEvents([FromQuery] int sinceEventId = 0, [FromQuery] int pageSize = 1000, [FromQuery] int pageNumber = 1, CancellationToken cancellationToken = default)
    {
        sinceEventId = sinceEventId < 0 ? 0 : sinceEventId;
        pageSize = pageSize is < 1 or > 1000 ? 1000 : pageSize;
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        GetOrganisationStatusEventsQuery query = new(sinceEventId, pageSize, pageNumber);
        GetOrganisationStatusEventsQueryResult result = await _mediator.Send(query, cancellationToken);
        return Ok(result.Events);
    }
}
