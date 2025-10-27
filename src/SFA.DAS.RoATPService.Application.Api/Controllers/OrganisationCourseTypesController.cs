using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Api.Common;
using SFA.DAS.RoATPService.Application.Api.Filters;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Common.Models;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("organisations")]
[Tags("Organisations")]
public class OrganisationCourseTypesController(IMediator _mediator, ILogger<OrganisationCourseTypesController> _logger) : ControllerBase
{
    [HttpPut]
    [Route("{ukprn}/course-types")]
    public async Task<IActionResult> UpdateCourseTypes([FromRoute] int ukprn, [FromBody] UpdateCourseTypesModel model, CancellationToken cancellationToken)
    {
        UpdateOrganisationCourseTypesCommand command = new(ukprn, model.CourseTypeIds, model.UserId);
        ValidatedResponse validatedResponse = await _mediator.Send(command, cancellationToken);
        return validatedResponse.IsValidResponse ? Ok() : new BadRequestObjectResult(validatedResponse.Errors);
    }

    [HttpDelete]
    [RequiredHeader(Constants.RequestingUserIdHeader)]
    [Route("{ukprn}/short-courses")]
    public async Task<IActionResult> DeleteShortCourseTypes([FromRoute] int ukprn, [FromHeader(Name = Constants.RequestingUserIdHeader)] string requestingUserId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request received to DeleteShortCourseTypes for ukprn {ukprn}.", ukprn);
        DeleteOrganisationShortCourseTypesCommand command = new(ukprn, requestingUserId);
        ValidatedResponse<SuccessModel> validatedResponse = await _mediator.Send(command, cancellationToken);
        if (!validatedResponse.IsValidResponse) return new BadRequestObjectResult(validatedResponse.Errors);
        return validatedResponse.Result.IsSuccess ? NoContent() : NotFound();
    }
}
