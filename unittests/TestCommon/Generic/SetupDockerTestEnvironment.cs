using System.Data;
using System.Data.Common;
using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace TestCommon.Generic;

[SetUpFixture]
public abstract class SetupDockerTestEnvironment
{
    protected abstract IGrateTestContext GrateTestContext { get; }
    protected abstract IDockerTestContext DockerTestContext { get; }
    private IContainer? _dockerContainer;
    private string? _serverName;
    //private string? _containerId;
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
        GrateTestContext.AdminPassword = _random.GetString(10, UpperCase) +
                                         _random.GetString(10, LowerCase) +
                                         _random.GetString(10, Digits);

        var builder = new ContainerBuilder()
        .WithImage(DockerTestContext.DockerImage)
        .WithPortBinding(DockerTestContext.ContainerPort ?? default, true)
        .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(DockerTestContext.WaitStrategy));
        builder = DockerTestContext.AddEnvironmentVariables(builder);
        _dockerContainer = builder.Build();
        await _dockerContainer.StartAsync();
        //await TestContext.Out.WriteAsync($"Starting {GrateTestContext.DatabaseTypeName} docker container: ");
        // (_containerId, var port) = await Docker.StartDockerContainer(_serverName, password, DockerTestContext.ContainerPort, DockerTestContext.DockerCommand);

        GrateTestContext.Port = _dockerContainer.GetMappedPublicPort(DockerTestContext.ContainerPort!.Value);
        await TestContext.Progress.WriteLineAsync($"Started {GrateTestContext.DatabaseTypeName} docker container: " + _dockerContainer.Id);
        await TestContext.Progress.WriteLineAsync("Listening on port: " + GrateTestContext.Port);

        await TestContext.Progress.WriteAsync("Waiting until server is ready");
        var ready = await WaitUntilServerIsReady();

        await TestContext.Progress.WriteLineAsync(ready ? "...ready." : "...gave up.");
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        //var containerId = await Docker.Delete(_containerId!);
        //await TestContext.Progress.WriteLineAsync($"Removed {GrateTestContext.DatabaseTypeName} docker container: " + containerId);
        await _dockerContainer!.StopAsync();
        await _dockerContainer.DisposeAsync();
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

        bool ready = false;

        try
        {
            await using var conn = GrateTestContext.CreateAdminDbConnection();
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;

            var res = (string?)await cmd.ExecuteScalarAsync();
            ready = res?.StartsWith(GrateTestContext.ExpectedVersionPrefix) ?? false;
        }
        catch (DbException) when (swallowException)
        {

        }

        return ready;
    }
}
