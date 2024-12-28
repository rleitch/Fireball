using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Fireball.FunctionApp
{
    public class Program
    {
        private static string _connectionString;

        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices((context, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();

                    _connectionString = context.Configuration.GetConnectionString("DistributedCacheConnection");
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = _connectionString;
                        options.SchemaName = "dbo";
                        options.TableName = "DistributedCache";
                    });
                })
                .Build();

            InitializeDatabase();

            host.Run();
        }

        private static void InitializeDatabase()
        {
            var sqlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "InitScript.sql");
            var script = File.ReadAllText(sqlFilePath);
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var command = new SqlCommand(script, connection);
            command.ExecuteNonQuery();
        }
    }
}