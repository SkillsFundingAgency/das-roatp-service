using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("api/v1/fat-data-export")]
public class FatDataExportController(ILogger<FatDataExportController> _logger, IFatDataExportService _service) : ControllerBase
{
    [HttpGet("")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FatDataExport>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    public async Task<IActionResult> DataExport()
    {
        _logger.LogInformation($"Received request for FAT data export");

        try
        {
            var result = await _service.GetData();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(
                $"Could not generate FAT data export due to : {ex.Message}");
            return BadRequest();
        }
    }
}