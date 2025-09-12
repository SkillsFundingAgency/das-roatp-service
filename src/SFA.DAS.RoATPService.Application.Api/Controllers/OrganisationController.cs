namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Domain;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using RoATPService.Api.Types.Models;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/[controller]")]
    public class OrganisationController : ControllerBase
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly IMediator _mediator;

        public OrganisationController(ILogger<OrganisationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Organisation))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
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
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEngagements(long sinceEventId = 0, int pageSize = 1000, int pageNumber = 1)
        {
            _logger.LogInformation($"Processing Organisation-GetEngagements, sinceEventId={sinceEventId}, pageSize={pageSize}, pageNumber={pageNumber}");

            var request = new GetEngagementsRequest { SinceEventId = sinceEventId, PageSize = pageSize, PageNumber = pageNumber };

            return Ok(await _mediator.Send(request));
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrganisationRequest createOrganisationRequest)
        {
            return Ok(await _mediator.Send(createOrganisationRequest));
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateOrganisationRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(OrganisationRegisterStatus))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("register-status")]
        public async Task<IActionResult> GetRegisterStatus(GetOrganisationRegisterStatusRequest getOrganisationRegisterStatusRequest)
        {
            return Ok(await _mediator.Send(getOrganisationRegisterStatusRequest));
        }

    }
}