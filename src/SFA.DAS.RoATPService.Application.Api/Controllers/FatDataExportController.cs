using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Api.Middleware;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;

namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    [ApiController]
    [Route("api/v1/fat-data-export")]
    public class FatDataExportController : ControllerBase
    {
        private readonly ILogger<FatDataExportController> _logger;
        private readonly IFatDataExportService _service;

        public FatDataExportController(ILogger<FatDataExportController> logger,
            IFatDataExportService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FatDataExport>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
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
}