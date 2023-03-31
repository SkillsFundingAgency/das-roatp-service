using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Domain.Models.FatDataExport;

namespace SFA.DAS.RoATPService.Application.Services
{
    public class FatDataExportService : IFatDataExportService
    {
        private readonly IFatDataExportRepository _repository;

        public FatDataExportService (IFatDataExportRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<FatDataExport>> GetData()
        {
            var data = await _repository.GetFatDataExport();

            var fatDataExports = data.Select(c=>(FatDataExport)c).ToList();

            return fatDataExports;
        }
    }
}