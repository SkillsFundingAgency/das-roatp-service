using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Application.Api.Middleware;
using SFA.DAS.RoATPService.Application.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "RoATPServiceInternalAPI, FATDataExport")]
    [Route("api/v1/fat-data-export")]
    public class FatDataExportController : ControllerBase
    {
        private readonly ILogger<FatDataExportController> _logger;
        private readonly IFatDataExportRepository _repository;

        public FatDataExportController(ILogger<FatDataExportController> logger,
            IFatDataExportRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(IEnumerable<object>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DataExport()
        {
            _logger.LogInformation($"Received request for FAT data export");

            try
            {
                var result = await _repository.GetFatDataExport();
                return Ok(result);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogInformation(
                    $"Could not generate FAT data export due to : {sqlEx.Message}");
                return NoContent();
            }
        }
    }
}