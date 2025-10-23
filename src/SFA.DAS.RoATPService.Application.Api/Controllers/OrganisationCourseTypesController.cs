using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Application.Api.Common;
using SFA.DAS.RoATPService.Application.Api.Filters;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.commands.DeleteOrganisationShortCourseTypes;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("organisations")]
[Tags("Organisations")]
public class OrganisationCourseTypesController(IMediator _mediator) : ControllerBase
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
        DeleteOrganisationShortCourseTypesCommand command = new(ukprn, requestingUserId);
        ValidatedResponse validatedResponse = await _mediator.Send(command, cancellationToken);
        return validatedResponse.IsValidResponse ? NoContent() : new BadRequestObjectResult(validatedResponse.Errors);
    }
}
