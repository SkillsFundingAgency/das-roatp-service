using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Interfaces;

public interface IDownloadRegisterRepository
{
    Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory();
}
