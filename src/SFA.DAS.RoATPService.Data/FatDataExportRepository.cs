namespace SFA.DAS.RoATPService.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using SFA.DAS.RoATPService.Domain.Models.FatDataExport;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class FatDataExportRepository : IFatDataExportRepository
    {
        private const string FatDataExportStoredProcedure = "[dbo].[RoATP_FAT_Data_Export]";

        private readonly IDbConnectionHelper _dbConnectionHelper;

        public FatDataExportRepository(IDbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
        }

        public async Task<IEnumerable<FatDataExportDto>> GetFatDataExport()
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                return (await connection.QueryAsync<FatDataExportDto>(FatDataExportStoredProcedure, 
                    commandType: CommandType.StoredProcedure)).ToList();
            }
        }
    }
}