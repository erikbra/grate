using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using moo.unittests.TestInfrastructure;
using NUnit.Framework;

namespace moo.unittests.SqlServer
{
    [SetUpFixture]
    public class SetupTestEnvironment
    {
        private string? _serverName;
        private string? _containerId;
    
        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "1234567890";

        private const string ServerNameAllowedChars = "abcdefghijklmnopqrstuvwxyz";

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            var random = new Random();
            
            _serverName = "moo-sqlserver-" + random.GetString(10, ServerNameAllowedChars);
            var password = 
                random.GetString(10, UpperCase) + 
                random.GetString(10, LowerCase) +
                random.GetString(10, Digits);
            
            //await TestContext.Out.WriteAsync("Starting SQL server docker container: ");
            int port;
            (_containerId, port) = await Docker.StartSqlServer(_serverName, password);
            
            MooTestContext.SqlServer.AdminPassword = password;
            MooTestContext.SqlServer.Port = port;
            
            await TestContext.Progress.WriteLineAsync("Started SQL server docker container: " + _containerId);
            await TestContext.Progress.WriteLineAsync("Listening on port: " + port);
            
            await TestContext.Progress.WriteAsync("Waiting until server is ready");
            var ready =  await WaitUntilServerIsReady();
            
            await TestContext.Progress.WriteLineAsync(ready ? "...ready." : "...gave up.");
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            //await TestContext.Progress.WriteAsync("Removing SQL server docker container: ");
            var containerId = await Docker.DeleteSqlServer(_containerId!);
            await TestContext.Progress.WriteLineAsync("Removed SQL server docker container: " + containerId);
        }

        private async Task<bool> WaitUntilServerIsReady()
        {
            var delay = 2000;
            var sleepTime = 0;
            var maxSleepTime = delay * 5;

            var ready = await ServerIsReady(true);

            while (!ready && sleepTime <= maxSleepTime)
            {
                await Task.Delay(delay);
                ready = await ServerIsReady(true);
                sleepTime += delay;
                await TestContext.Progress.WriteAsync(".");
            }

            // Try one last time, and just fail with an exception if it still fails
            if (!ready)
            {
                ready = await ServerIsReady(false);
            }

            return ready;
        }

        private static async Task<bool> ServerIsReady(bool swallowException)
        {
            var db = "master";
            //var pw = "LYFDIDKQULvrurqwakee1666029582";
            //var port = 49165;
            
            var pw = MooTestContext.SqlServer.AdminPassword;
            var port = MooTestContext.SqlServer.Port;
        

            var connectionString = $"Data Source=localhost,{port};Initial Catalog={db};User Id=sa;Password={pw}";
            var sql = "SELECT @@VERSION";

            string? res;

            bool ready = false;

            try
            {
                await using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                res = (string?) await cmd.ExecuteScalarAsync();
                ready = res?.StartsWith("Microsoft SQL Server 2017") ?? false;
            }
            catch (SqlException) when (swallowException)
            {
                
            }

            return ready;
        }
    }
}