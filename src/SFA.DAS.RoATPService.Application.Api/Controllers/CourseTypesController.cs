using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Queries.GetCourseTypes;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;
[Route("course-types")]
[ApiController]
public class CourseTypesController(IMediator _mediator, ILogger<CourseTypesController> _logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCourseTypesResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public async Task<IActionResult> GetCourseTypes(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing CourseTypes-GetCourseTypes");
        GetCourseTypesResult result = await _mediator.Send(new GetCourseTypesQuery(), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }
}