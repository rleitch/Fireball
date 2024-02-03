using Fireball.FunctionApp.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using System.IO;

[assembly: FunctionsStartup(typeof(Fireball.FunctionApp.Startup))]
namespace Fireball.FunctionApp
{
    public class Startup : FunctionsStartup
    {
        private string _connectionString;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            _connectionString = configuration.GetConnectionString("DistributedCacheConnection");
            InitializeDatabase();
            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = _connectionString;
                options.SchemaName = "dbo";
                options.TableName = "DistributedCache";
            });
            builder.Services.AddOptions<Settings>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(Settings)).Bind(settings);
                });
        }

        private void InitializeDatabase()
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