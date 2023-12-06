using System;
using System.Data.Common;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.TestInfrastructure;

public class PostgreSqlGrateTestContext : TestContextBase, IGrateTestContext, IDockerTestContext
{
    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }

    // public string DockerCommand(string serverName, string adminPassword) =>
    //     $"run -d --name {serverName} -e POSTGRES_PASSWORD={adminPassword} -P postgres:latest";
    public override int? ContainerPort => 5432;

    public string AdminConnectionString =>
        $"Host=localhost;Port={Port};Database=postgres;Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public string ConnectionString(string database) =>
        $"Host=localhost;Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public string UserConnectionString(string database) =>
        $"Host=localhost;Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public DbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);

    public ISyntax Syntax => new PostgreSqlSyntax();
    public Type DbExceptionType => typeof(PostgresException);

    public DatabaseType DatabaseType => DatabaseType.postgresql;
    public bool SupportsTransaction => true;
    public string DatabaseTypeName => "PostgreSQL";
    public string MasterDatabase => "postgres";

    public IDatabase DatabaseMigrator => new PostgreSqlDatabase(TestConfig.LogFactory.CreateLogger<PostgreSqlDatabase>());

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT version()",
        SleepTwoSeconds = "SELECT pg_sleep(2);"
    };


    public string ExpectedVersionPrefix => "PostgreSQL 16.";
    public bool SupportsCreateDatabase => true;
    public string? DockerImage => "postgres:latest";

    public ContainerBuilder AddEnvironmentVariables(ContainerBuilder builder)
    {
        return builder.WithEnvironment("POSTGRES_PASSWORD", AdminPassword);
    }
    public IWaitUntil WaitStrategy => new WaitUntiPostgreSQLReady();

    private sealed class WaitUntiPostgreSQLReady : IWaitUntil
    {
        private static readonly string[] s_lineEndings =
        {
            "\r\n", "\n"
        };

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, stderr) = await container.GetLogsAsync(timestampsEnabled: false)
                .ConfigureAwait(false);

            return 2.Equals(Array.Empty<string>()
                .Concat(stdout.Split(s_lineEndings, StringSplitOptions.RemoveEmptyEntries))
                .Concat(stderr.Split(s_lineEndings, StringSplitOptions.RemoveEmptyEntries))
                .Count(line => line.Contains("database system is ready to accept connections")));
        }
    }
}
