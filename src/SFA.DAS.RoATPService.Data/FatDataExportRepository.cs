using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Data
{
    public class FatDataExportRepository : IFatDataExportRepository
    {
        private readonly IWebConfiguration _configuration;
        private const string FatDataExportStoredProcedure = "[dbo].[RoATP_FAT_Data_Export]";

        public FatDataExportRepository(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<object>> GetFatDataExport()
        {
            using (var connection = new SqlConnection(_configuration.SqlConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return (await connection.QueryAsync(FatDataExportStoredProcedure, 
                    commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }
        }
    }
}