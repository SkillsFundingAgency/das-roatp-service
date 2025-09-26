using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;

namespace SFA.DAS.RoATPService.Infrastructure.Database;

public class DbConnectionHelper : IDbConnectionHelper
{
    private readonly string _sqlConnectionString;

    public DbConnectionHelper(IConfiguration configuration)
    {
        _sqlConnectionString = configuration["SqlConnectionString"];
    }

    public IDbConnection GetDatabaseConnection()
    {
        var connection = new SqlConnection(_sqlConnectionString);

        return connection;
    }
}
