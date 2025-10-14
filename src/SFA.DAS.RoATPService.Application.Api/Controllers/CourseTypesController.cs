using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;
[Route("course-types")]
[ApiController]
public class CourseTypesController(IMediator _mediator, ILogger<CourseTypesController> _logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllCourseTypesResult))]

    public async Task<IActionResult> GetAllCourseTypes(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing CourseTypes-GetCourseTypes");
        GetAllCourseTypesResult result = await _mediator.Send(new GetAllCourseTypesQuery(), cancellationToken);
        return Ok(result);
    }
}