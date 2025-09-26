using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Api.Types.Models;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SearchController(IMediator _mediator, ILogger<SearchController> _logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<OrganisationSearchResults>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    public async Task<IActionResult> Index([FromQuery] string searchTerm)
    {
        _logger.LogInformation("Received request to search for organisations with search term: {SearchTerm}", searchTerm);
        OrganisationSearchRequest searchQuery = new() { SearchTerm = searchTerm };
        return Ok(await _mediator.Send(searchQuery));
    }
}
