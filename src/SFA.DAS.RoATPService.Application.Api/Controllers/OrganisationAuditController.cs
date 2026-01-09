using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisationAuditRecords;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("organisations")]
[Tags("Organisations")]
public class OrganisationAuditController(ILogger<OrganisationAuditController> _logger, IMediator _mediator) : ControllerBase
{
    [HttpGet("audit-records")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(GetOrganisationAuditRecordsQueryResult))]
    public async Task<IActionResult> GetFullAuditHistory(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to download register audit history");

        var result = await _mediator.Send(new GetOrganisationAuditRecordsQuery(), cancellationToken);
        return Ok(result);
    }
}
