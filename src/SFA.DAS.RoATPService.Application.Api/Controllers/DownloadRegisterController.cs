using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SFA.DAS.RoATPService.Application.Api.Helpers;
using SFA.DAS.RoATPService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[ApiController]
[Route("api/v1/download")]
public class DownloadRegisterController(ILogger<DownloadRegisterController> _logger, IDownloadRegisterRepository _repository, IDataTableHelper _dataTableHelper) : ControllerBase
{
    [HttpGet("complete")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    public async Task<IActionResult> CompleteRegister()
    {
        _logger.LogInformation("Received request to download complete register");

        try
        {
            var result = await _repository.GetCompleteRegister();
            return Ok(result);
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Could not generate data for complete register download due to : {ErrorMessage}", sqlEx.Message);
            return NoContent();
        }
    }

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

    [HttpGet("roatp-summary")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    public async Task<IActionResult> RoatpSummary()
    {
        _logger.LogInformation("Received request to download roatp summary");

        try
        {
            return Ok(await _repository.GetRoatpSummary());
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Could not generate data for roatp summary due to : {ErrorMessage}", sqlEx.Message);
            return NoContent();
        }
    }


    [HttpGet("roatp-summary/{ukprn}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<IDictionary<string, object>>))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
    public async Task<IActionResult> RoatpSummary(string ukprn)
    {
        _logger.LogInformation("Received request to download roatp summary for ukprn {Ukprn}", ukprn);

        int ukprnAsInt = 0;

        if (!int.TryParse(ukprn, out ukprnAsInt) || (ukprnAsInt < 10000000 || ukprnAsInt > 99999999))
        {
            _logger.LogError("Could not generate data for invalid ukprn : {Ukprn}", ukprn);
            return BadRequest();
        }

        try
        {
            return Ok(await _repository.GetRoatpSummaryUkprn(ukprnAsInt));
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Could not generate data for roatp summary due to database error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


    [HttpGet("roatp-summary/most-recent")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(DateTime?))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(DateTime?))]
    public async Task<IActionResult> MostRecentOrganisation()
    {
        _logger.LogInformation("Received request to get date of most recent non-onboarding organisation change");

        try
        {
            return Ok(await _repository.GetLatestNonOnboardingOrganisationChangeDate());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not generate data for latest organisation date change due to database or other internal error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("roatp-summary-xlsx")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FileContentResult))]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<IActionResult> RoatpSummaryExcel()
    {
        _logger.LogInformation("Received request to download complete register xlsx");

        try
        {
            var resultsSummary = await _repository.GetRoatpSummary();
            ExcelPackage.License.SetNonCommercialOrganization("Department for Education");
            using (var package = new ExcelPackage())
            {
                var worksheetToAdd = package.Workbook.Worksheets.Add("RoATP");
                worksheetToAdd.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(resultsSummary), true);
                return File(package.GetAsByteArray(), "application/excel", $"roatp.xlsx");
            }
        }
        catch (SqlException sqlEx)
        {
            _logger.LogError(sqlEx, "Could not generate data for roatp summary due to : {ExceptionMessage}", sqlEx.Message);
            return NoContent();
        }
    }
}