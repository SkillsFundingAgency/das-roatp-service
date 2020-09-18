using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;

namespace SFA.DAS.RoATPService.Application.Interfaces
{
    public interface IFatDataExportRepository
    {
        Task<IEnumerable<FatDataExportDto>> GetFatDataExport();
    }
}