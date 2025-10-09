using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.Queries.GetOrganisation;

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

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatchOrganisationModel))]
    public IActionResult PatchOrganization([FromBody] PatchOrganisationModel model)
    {

        return Ok(new { HasLegalName = model.ProviderType.HasValue, HasOrganidationId = model.OrganisationTypeId.HasValue, HasStatus = model.Status.HasValue, HasIncorpDate = model.ReasonForRemoval.HasValue });
    }
}

