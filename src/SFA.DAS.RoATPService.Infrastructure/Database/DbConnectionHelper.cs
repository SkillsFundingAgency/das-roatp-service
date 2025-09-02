using System.Data;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;
using SFA.DAS.RoATPService.Settings;

namespace SFA.DAS.RoATPService.Infrastructure.Database
{
    public class DbConnectionHelper : IDbConnectionHelper
    {
        private readonly WebConfiguration _configuration;

        public DbConnectionHelper(WebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetDatabaseConnection()
        {
            var connectionString = _configuration.SqlConnectionString;

            var connection = new System.Data.SqlClient.SqlConnection(connectionString);

            return connection;
        }
    }
}
