using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace grate.unittests.TestInfrastructure;

internal class PostgreSqlGrateTestContext : TestContextBase, IGrateTestContext, IDockerTestContext
{
    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }

    public string DockerCommand(string serverName, string adminPassword) =>
        $"run -d --name {serverName} -e POSTGRES_PASSWORD={adminPassword} -P postgres:latest";

    public string AdminConnectionString => $"Host=localhost;Port={Port};Database=postgres;Username=postgres;Password={AdminPassword};Include Error Detail=true";
    public string ConnectionString(string database) => $"Host=localhost;Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true";

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


    public string ExpectedVersionPrefix => "PostgreSQL 14.";
    public bool SupportsCreateDatabase => true;
    public bool DatabaseNamingCaseSensitive => true;
}
