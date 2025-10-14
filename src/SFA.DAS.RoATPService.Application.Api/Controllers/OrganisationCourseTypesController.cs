﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Application.Api.Models;
using SFA.DAS.RoATPService.Application.Commands.UpdateOrganisationCourseTypes;
using SFA.DAS.RoATPService.Application.Mediatr.Behaviors;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("organisations/{ukprn}/course-types")]
public class OrganisationCourseTypesController(IMediator _mediator) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> UpdateCourseTypes([FromRoute] int ukprn, [FromBody] UpdateCourseTypesModel model, CancellationToken cancellationToken)
    {
        UpdateOrganisationCourseTypesCommand command = new(ukprn, model.CourseTypeIds, model.UserId);
        ValidatedResponse validatedResponse = await _mediator.Send(command, cancellationToken);
        return validatedResponse.IsValidResponse ? Ok() : new BadRequestObjectResult(validatedResponse.Errors);
    }
}
