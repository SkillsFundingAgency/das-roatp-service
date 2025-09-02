namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.RoATPService.Api.Types.Models;
    using SFA.DAS.RoATPService.Application.Api.Middleware;

    [ApiController]
    [Route("api/v1/duplicateCheck")]
    public class DuplicateCheckController : ControllerBase
    {
        private ILogger<LookupDataController> _logger;

        private IMediator _mediator;

        public DuplicateCheckController(ILogger<LookupDataController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("ukprn")]
        public async Task<IActionResult> UKPRN(DuplicateUKPRNCheckRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("companyNumber")]
        public async Task<IActionResult> CompanyNumber(DuplicateCompanyNumberCheckRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(bool))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("charityNumber")]
        public async Task<IActionResult> CharityNumber(DuplicateCharityNumberCheckRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
