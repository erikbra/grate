using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.TestInfrastructure;

public class PostgreSqlGrateTestContext : IGrateTestContext
{
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly PostgresqlTestContainer _testContainer;
    public PostgreSqlGrateTestContext(IServiceProvider serviceProvider, PostgresqlTestContainer container)
    {
        ServiceProvider = serviceProvider;
        _testContainer = container;
    }
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);


    public string AdminConnectionString =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database=postgres;Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public string ConnectionString(string database) =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public string UserConnectionString(string database) =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public DbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);

    public ISyntax Syntax => new PostgreSqlSyntax();
    public Type DbExceptionType => typeof(PostgresException);

    public DatabaseType DatabaseType => DatabaseType.postgresql;
    public bool SupportsTransaction => true;
    public string DatabaseTypeName => "PostgreSQL";
    public string MasterDatabase => "postgres";

    public IDatabase DatabaseMigrator => new PostgreSqlDatabase(ServiceProvider.GetRequiredService<ILogger<PostgreSqlDatabase>>());

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT version()",
        SleepTwoSeconds = "SELECT pg_sleep(2);"
    };

    public string ExpectedVersionPrefix => "PostgreSQL 16.";
    public bool SupportsCreateDatabase => true;

}
