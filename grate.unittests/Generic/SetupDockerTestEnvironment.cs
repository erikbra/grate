using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic
{
    [SetUpFixture]
    public abstract class SetupDockerTestEnvironment
    {
        protected abstract IGrateTestContext GrateTestContext { get; }
        protected abstract IDockerTestContext DockerTestContext { get; }

        private string? _serverName;
        private string? _containerId;
        private readonly Random _random = new();

        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "1234567890";

        private const string ServerNameAllowedChars = "abcdefghijklmnopqrstuvwxyz";

        private string GetServerName() =>
            $"grate-{GrateTestContext.DatabaseType}-{_random.GetString(10, ServerNameAllowedChars)}";

        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            
            _serverName = GetServerName();
            var password =
                _random.GetString(10, UpperCase) +
                _random.GetString(10, LowerCase) +
                _random.GetString(10, Digits);

            //await TestContext.Out.WriteAsync($"Starting {GrateTestContext.DatabaseTypeName} docker container: ");
            int port;
            (_containerId, port) = await Docker.StartDockerContainer(_serverName, password, DockerTestContext.DockerCommand);

            GrateTestContext.AdminPassword = password;
            GrateTestContext.Port = port;

            await TestContext.Progress.WriteLineAsync($"Started {GrateTestContext.DatabaseTypeName} docker container: " + _containerId);
            await TestContext.Progress.WriteLineAsync("Listening on port: " + port);

            await TestContext.Progress.WriteAsync("Waiting until server is ready");
            var ready = await WaitUntilServerIsReady();

            await TestContext.Progress.WriteLineAsync(ready ? "...ready." : "...gave up.");
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            var containerId = await Docker.Delete(_containerId!);
            await TestContext.Progress.WriteLineAsync($"Removed {GrateTestContext.DatabaseTypeName} docker container: " + containerId);
            Trace.Flush();
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

        private async Task<bool> ServerIsReady(bool swallowException)
        {
            var sql = GrateTestContext.Sql.SelectVersion;

            string? res;

            bool ready = false;

            try
            {
                await using var conn = GrateTestContext.CreateAdminDbConnection();
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                res = (string?)await cmd.ExecuteScalarAsync();
                ready = res?.StartsWith(GrateTestContext.ExpectedVersionPrefix) ?? false;
            }
            catch (DbException) when (swallowException)
            {

            }

            return ready;
        }
    }
}
