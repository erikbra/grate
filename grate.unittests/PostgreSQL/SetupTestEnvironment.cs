using System;
using System.Data;
using System.Threading.Tasks;
using grate.unittests.TestInfrastructure;
using Npgsql;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL
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
            
            _serverName = "grate-postgresql-" + random.GetString(10, ServerNameAllowedChars);
            var password = 
                random.GetString(10, UpperCase) + 
                random.GetString(10, LowerCase) +
                random.GetString(10, Digits);
            
            //await TestContext.Out.WriteAsync("Starting PostgreSQL docker container: ");
            int port;
            (_containerId, port) = await Docker.StartPostgreSQL(_serverName, password);
            
            GrateTestContext.PostgreSql.AdminPassword = password;
            GrateTestContext.PostgreSql.Port = port;
            
            await TestContext.Progress.WriteLineAsync("Started PostgreSQL docker container: " + _containerId);
            await TestContext.Progress.WriteLineAsync("Listening on port: " + port);
            
            await TestContext.Progress.WriteAsync("Waiting until server is ready");
            var ready =  await WaitUntilServerIsReady();
            
            await TestContext.Progress.WriteLineAsync(ready ? "...ready." : "...gave up.");
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            //await TestContext.Progress.WriteAsync("Removing PostgreSQL docker container: ");
            var containerId = await Docker.Delete(_containerId!);
            await TestContext.Progress.WriteLineAsync("Removed PostgreSQL docker container: " + containerId);
        }

        private async Task<bool> WaitUntilServerIsReady()
        {
            var delay = 2000;
            var sleepTime = 0;
            var maxSleepTime = delay * 20;

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
            var db = "postgresql";
            //var pw = "LYFDIDKQULvrurqwakee1666029582";
            //var port = 49165;
            
            var pw = GrateTestContext.PostgreSql.AdminPassword;
            var port = GrateTestContext.PostgreSql.Port;
        

            var connectionString = $"Host=localhost;Port={port};Database={db};Username=postgres;Password={pw}";
            var sql = "SELECT @@VERSION";

            string? res;

            bool ready = false;

            try
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                res = (string?) await cmd.ExecuteScalarAsync();
                ready = res?.StartsWith("Microsoft PostgreSQL 2017") ?? false;
            }
            catch (NpgsqlException) when (swallowException)
            {
                
            }

            return ready;
        }
    }
}