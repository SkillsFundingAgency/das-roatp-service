using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Api.Types.Models;
using SFA.DAS.RoATPService.Domain;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("api/v1/lookupData")]
[ApiController]
public class LookupDataController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProviderType>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("providerTypes")]
    public async Task<IActionResult> ProviderTypes()
    {
        var request = new GetProviderTypesRequest();

        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrganisationType>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("organisationTypes")]
    public async Task<IActionResult> OrganisationTypes(int providerTypeId)
    {
        var request = new GetOrganisationTypesRequest { ProviderTypeId = providerTypeId };

        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrganisationCategory>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("organisationCategories/{providerTypeId}")]
    public async Task<IActionResult> OrganisationCategories(int providerTypeId)
    {
        var request = new GetOrganisationCategoriesRequest { ProviderTypeId = providerTypeId };

        return Ok(await _mediator.Send(request));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="providerTypeId"></param>
    /// <param name="categoryId"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrganisationType>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("organisationTypes/{providerTypeId}/{categoryId}")]
    public async Task<IActionResult> OrganisationTypesByCategory(int providerTypeId, int categoryId)
    {
        var request = new GetOrganisationTypesByCategoryRequest { ProviderTypeId = providerTypeId, CategoryId = categoryId };

        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrganisationStatus>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("organisationStatuses")]
    public async Task<IActionResult> OrganisationStatuses(int? providerTypeId)
    {
        var request = new GetOrganisationStatusesRequest { ProviderTypeId = providerTypeId };
        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<RemovedReason>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    [Route("removedReasons")]
    public async Task<IActionResult> RemovedReasons()
    {
        var request = new GetRemovedReasonsRequest();

        return Ok(await _mediator.Send(request));
    }
}