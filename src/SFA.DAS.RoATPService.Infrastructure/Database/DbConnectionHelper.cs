using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.RoATPService.Infrastructure.Interfaces;
using SFA.DAS.RoATPService.Settings;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Infrastructure.Database
{
    public class DbConnectionHelper : IDbConnectionHelper
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DbConnectionHelper(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IDbConnection GetDatabaseConnection()
        {
            var connectionString = _configuration.SqlConnectionString;

            var connection = new System.Data.SqlClient.SqlConnection(connectionString);

            if (!_hostingEnvironment.IsDevelopment())
            {
                var generateTokenTask = GenerateTokenAsync();
                connection.AccessToken = generateTokenTask.GetAwaiter().GetResult();
            }

            return connection;
        }

        private static async Task<string> GenerateTokenAsync()
        {
            const string AzureResource = "https://database.windows.net/";

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(AzureResource);

            return accessToken;
        }
    }
}
