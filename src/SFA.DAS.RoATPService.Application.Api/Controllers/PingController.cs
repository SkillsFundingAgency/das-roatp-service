using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[AllowAnonymous]
public class PingController : ControllerBase
{
    [HttpGet("/Ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
}