using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.RoATPService.Application.Interfaces;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;

namespace SFA.DAS.RoATPService.Data;

public class DownloadRegisterRepository : IDownloadRegisterRepository
{
    private const string AuditHistoryStoredProcedure = "[dbo].[RoATP_Audit_History]";

    private readonly IDbConnectionHelper _dbConnectionHelper;

    public DownloadRegisterRepository(IDbConnectionHelper dbConnectionHelper)
    {
        _dbConnectionHelper = dbConnectionHelper;
    }

    public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
    {
        using (var connection = _dbConnectionHelper.GetDatabaseConnection())
        {
            return (await connection.QueryAsync(AuditHistoryStoredProcedure,
                commandType: CommandType.StoredProcedure)).OfType<IDictionary<string, object>>().ToList();
        }
    }
}
