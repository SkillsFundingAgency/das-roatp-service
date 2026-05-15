using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoATPService.Ukrlp.Client;

namespace SFA.DAS.RoATPService.Application.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController(IUkrlpService _ukrlpService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Provider>> Get(int count, CancellationToken cancellationToken)
    {
        var ukprns = Enumerable.Range(10000001, count);
        var provider = await _ukrlpService.GetProviderDataAsync(ukprns, cancellationToken);
        return Ok(provider);
    }
}
