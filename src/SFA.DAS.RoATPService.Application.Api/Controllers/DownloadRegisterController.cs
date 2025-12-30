using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Interfaces;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("api/v1/download")]
public class DownloadRegisterController(ILogger<DownloadRegisterController> _logger, IDownloadRegisterRepository _repository) : ControllerBase
{
    [HttpGet("audit")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    public async Task<IActionResult> AuditHistory()
    {
        _logger.LogInformation("Received request to download register audit history");

        try
        {
            var result = await _repository.GetAuditHistory();
            return Ok(result);
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Could not generate data for register audit history due to : {ErrorMessage}", sqlEx.Message);
            return NoContent();
        }
    }
}
