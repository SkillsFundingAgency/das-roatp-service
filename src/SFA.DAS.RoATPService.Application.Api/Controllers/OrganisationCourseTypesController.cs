using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Common.Models;
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
        ValidatedResponse<SuccessModel> validatedResponse = await _mediator.Send(command, cancellationToken);
        if (!validatedResponse.IsValidResponse) return new BadRequestObjectResult(validatedResponse.Errors);
        return validatedResponse.Result.IsSuccess ? NoContent() : NotFound();
    }
}
