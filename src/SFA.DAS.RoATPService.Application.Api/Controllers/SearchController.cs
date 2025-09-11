namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using RoATPService.Api.Types.Models;

    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IMediator _mediator;

        public SearchController(ILogger<SearchController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<OrganisationSearchRequest>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Index(OrganisationSearchRequest searchQuery)
        {
            return Ok(await _mediator.Send(searchQuery));
        }
    }
}