using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    public interface IFatDataExportRepository
    {
        Task<IEnumerable<object>> GetFatDataExport();
    }
}