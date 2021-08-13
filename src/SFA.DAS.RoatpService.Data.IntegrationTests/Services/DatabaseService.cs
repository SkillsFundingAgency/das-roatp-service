using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;
using SFA.DAS.RoATPService.Infrastructure.Database;

namespace SFA.DAS.RoatpService.Data.IntegrationTests.Services
{
    public class DatabaseService
    {
        public DatabaseService()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("connectionStrings.Local.json")
                .Build();

            WebConfiguration = new TestWebConfiguration
            {
                SqlConnectionString = Configuration.GetConnectionString("SqlConnectionStringTest")
            };

            DbConnectionHelper = new DbConnectionHelper(WebConfiguration, new TestHostingEnvironment());
        }

        private readonly IConfiguration Configuration;
        private readonly TestWebConfiguration WebConfiguration;
        public readonly DbConnectionHelper DbConnectionHelper;

        private string _dbName => Configuration["DatabaseName"];
        private string _testDbName => Configuration["TestDatabaseName"];

        public void SetupDatabase()
        {
            DropDatabase();

            using (var connection = DbConnectionHelper.GetDatabaseConnection())
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection as SqlConnection,
                    CommandText =
                        $@"DBCC CLONEDATABASE ('{_dbName}', '{_testDbName}'); ALTER DATABASE [{_testDbName}] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }

        public void Execute(string sql)
        {
            using (var connection = DbConnectionHelper.GetDatabaseConnection())
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql);
                connection.Close();
            }
        }

        public T Get<T>(string sql)
        {
            using (var connection = DbConnectionHelper.GetDatabaseConnection())
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.Query<T>(sql);
                connection.Close();
                return result.FirstOrDefault();
            }
        }

        public object ExecuteScalar(string sql)
        {
            using (var connection = DbConnectionHelper.GetDatabaseConnection())
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var result = connection.ExecuteScalar(sql);
                connection.Close();

                return result;
            }
        }
        
        public void Execute(string sql, TestModel model)
        {
            using (var connection = DbConnectionHelper.GetDatabaseConnection())
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql, model);
                connection.Close();
            }
        }

        public void DropDatabase()
        {
            using (var connection = DbConnectionHelper.GetDatabaseConnection())
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection as SqlConnection,
                    CommandText =
                        $@"if exists(select * from sys.databases where name = '{_testDbName}') BEGIN ALTER DATABASE [{_testDbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE [{_testDbName}]; END"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }
    }
}
