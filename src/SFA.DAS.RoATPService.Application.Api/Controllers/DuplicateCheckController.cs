using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Api.Types.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;


[ApiController]
[Route("api/v1/duplicateCheck")]
public class DuplicateCheckController(IMediator _mediator) : ControllerBase
{

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DuplicateCheckResponse))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("ukprn")]
    public async Task<IActionResult> UKPRN([FromQuery] long ukprn, [FromQuery] Guid organisationId)
    {
        DuplicateUkprnCheckRequest request = new DuplicateUkprnCheckRequest { UKPRN = ukprn, OrganisationId = organisationId };
        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DuplicateCheckResponse))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("companyNumber")]
    public async Task<IActionResult> CompanyNumber([FromQuery] string companyNumber, [FromQuery] Guid organisationId)
    {
        DuplicateCompanyNumberCheckRequest request = new DuplicateCompanyNumberCheckRequest { CompanyNumber = companyNumber, OrganisationId = organisationId };
        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DuplicateCheckResponse))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("charityNumber")]
    public async Task<IActionResult> CharityNumber([FromQuery] string charityNumber, [FromQuery] Guid organisationId)
    {
        DuplicateCharityNumberCheckRequest request = new DuplicateCharityNumberCheckRequest { CharityNumber = charityNumber, OrganisationId = organisationId };
        return Ok(await _mediator.Send(request));
    }
}
