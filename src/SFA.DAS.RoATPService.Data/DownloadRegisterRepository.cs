namespace SFA.DAS.RoATPService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using SFA.DAS.RoATPService.Application.Interfaces;
    using SFA.DAS.RoATPService.Infrastructure.Interfaces;

    public class DownloadRegisterRepository : IDownloadRegisterRepository
    {
        private const string CompleteRegisterStoredProcedure = "[dbo].[RoATP_Complete_Register]";
        private const string AuditHistoryStoredProcedure = "[dbo].[RoATP_Audit_History]";
        private const string RoatpCsvSummary = "[dbo].[RoATP_CSV_SUMMARY]";

        private readonly IDbConnectionHelper _dbConnectionHelper;

        public DownloadRegisterRepository(IDbConnectionHelper dbConnectionHelper)
        {
            _dbConnectionHelper = dbConnectionHelper;
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                return (await connection.QueryAsync(CompleteRegisterStoredProcedure, 
                    commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                return (await connection.QueryAsync(AuditHistoryStoredProcedure,
                    commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetRoatpSummary()
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                return (await connection.QueryAsync(RoatpCsvSummary, commandType: CommandType.StoredProcedure))
                    .OfType<IDictionary<string, object>>().ToList();
            }
        }
        
        public async Task<IEnumerable<IDictionary<string, object>>> GetRoatpSummaryUkprn(int ukprn)
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {                
                return (await connection.QueryAsync(RoatpCsvSummary, new {ukprn},
                    commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
            }
        }

        public async Task<DateTime?> GetLatestNonOnboardingOrganisationChangeDate()
        {
            using (var connection = _dbConnectionHelper.GetDatabaseConnection())
            {
                const string sql = 
                    "select max(coalesce(updatedAt,createdAt)) latestDate from organisations o WHERE o.StatusId IN (0,1,2) ";
                return await connection.ExecuteScalarAsync<DateTime?>(sql);
            }
        }
    }
}
