using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Api.Common;
using SFA.DAS.RoATPService.Application.Api.Filters;
using SFA.DAS.RoATPService.Application.Commands.PatchOrganisation;
using SFA.DAS.RoATPService.Application.Commands.PostOrganisation;
using SFA.DAS.RoATPService.Application.Common;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisations;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class OrganisationsController(IMediator _mediator, ILogger<OrganisationsController> _logger) : ControllerBase
{
    [HttpGet]
    [Route("{ukprn:int}", Name = "GetOrganisationByUkprn")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrganisationModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisationByUkprn([FromRoute] int ukprn, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Organisations-GetOrganisationByUkprn for UKPRN {Ukprn}", ukprn);
        GetOrganisationQuery query = new(ukprn);
        OrganisationModel result = await _mediator.Send(query, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrganisationsQueryResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrganisations([FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Organisations-GetOrganisations");
        GetOrganisationsQueryResult result = await _mediator.Send(new GetOrganisationsQuery(searchTerm), cancellationToken);
        return Ok(result);
    }

    [HttpPatch]
    [Route("{ukprn:int}")]
    [RequiredHeader(Constants.RequestingUserIdHeader)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IEnumerable<ValidationError>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchOrganisation([FromRoute] int ukprn, [FromBody] JsonPatchDocument<PatchOrganisationModel> patchDoc, [FromHeader(Name = Common.Constants.RequestingUserIdHeader)] string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Organisations-PatchOrganisations");

        ValidatedResponse<SuccessModel> result = await _mediator.Send(new PatchOrganisationCommand(ukprn, userId, patchDoc), cancellationToken);

        if (!result.IsValidResponse) return BadRequest(result.Errors);

        return result.Result.IsSuccess ? NoContent() : NotFound();
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("")]
    public async Task<IActionResult> PostOrganisation([FromBody] PostOrganisationCommand command,
        CancellationToken cancellationToken)
    {
        ValidatedResponse<SuccessModel> validatedResponse = await _mediator.Send(command, cancellationToken);
        if (!validatedResponse.IsValidResponse) return new BadRequestObjectResult(validatedResponse.Errors);

        return Created(Url.RouteUrl("GetOrganisationByUkprn", new { ukprn = command.Ukprn }), null);
    }
}
