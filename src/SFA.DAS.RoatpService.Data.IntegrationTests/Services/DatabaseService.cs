using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;
using SFA.DAS.RoatpService.Data.IntegrationTests.Models;

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

            _dbName = Configuration["DatabaseName"];
            _testDbName = Configuration["TestDatabaseName"];
        }

        private IConfiguration Configuration { get; }
        private readonly string _dbName;
        private readonly string _testDbName;
        public readonly TestWebConfiguration WebConfiguration;

        public void SetupDatabase()
        {
            DropDatabase();

            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionString")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"DBCC CLONEDATABASE ('{_dbName}', '{_testDbName}'); ALTER DATABASE [{_testDbName}] SET READ_WRITE;"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }

        public void Execute(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql);
                connection.Close();
            }
        }

        public T Get<T>(string sql)
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
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
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
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
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionStringTest")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                connection.Execute(sql, model);
                connection.Close();
            }
        }

        public void DropDatabase()
        {
            using (var connection = new SqlConnection(Configuration.GetConnectionString("SqlConnectionString")))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var comm = new SqlCommand
                {
                    Connection = connection,
                    CommandText =
                        $@"if exists(select * from sys.databases where name = '{_testDbName}') BEGIN ALTER DATABASE [{_testDbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;  DROP DATABASE [{_testDbName}]; END"
                };
                var reader = comm.ExecuteReader();
                reader.Close();
            }
        }
    }
}
