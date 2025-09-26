using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Domain;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("api/v1/[controller]")]
public class OrganisationController(ILogger<OrganisationController> _logger, IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Organisation))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("get/{organisationId}")]
    public async Task<IActionResult> Get(Guid organisationId)
    {
        GetOrganisationRequest getOrganisationRequest = new GetOrganisationRequest { OrganisationId = organisationId };

        Organisation organisation = await _mediator.Send(getOrganisationRequest);

        return Ok(organisation);
    }

    [HttpGet]
    [Route("engagements")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Engagement>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IEnumerable<Engagement>))]
    public async Task<IActionResult> GetEngagements(long sinceEventId = 0, int pageSize = 1000, int pageNumber = 1)
    {
        _logger.LogInformation($"Processing Organisation-GetEngagements, sinceEventId={sinceEventId}, pageSize={pageSize}, pageNumber={pageNumber}");

        var request = new GetEngagementsRequest { SinceEventId = sinceEventId, PageSize = pageSize, PageNumber = pageNumber };

        return Ok(await _mediator.Send(request));
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Guid?))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrganisationRequest createOrganisationRequest)
    {
        return Ok(await _mediator.Send(createOrganisationRequest));
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateOrganisationRequest request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrganisationRegisterStatus))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("register-status")]
    public async Task<IActionResult> GetRegisterStatus(GetOrganisationRegisterStatusRequest getOrganisationRegisterStatusRequest)
    {
        return Ok(await _mediator.Send(getOrganisationRegisterStatusRequest));
    }
}