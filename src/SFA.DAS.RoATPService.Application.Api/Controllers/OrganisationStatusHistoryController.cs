using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationStatusHistory;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("organisations/{ukprn}/status-history")]
[Tags("Organisations")]
public class OrganisationStatusHistoryController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrganisationStatusHistoryQueryResult))]
    public async Task<IActionResult> GetAll([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        GetOrganisationStatusHistoryQuery query = new(ukprn);
        GetOrganisationStatusHistoryQueryResult result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
