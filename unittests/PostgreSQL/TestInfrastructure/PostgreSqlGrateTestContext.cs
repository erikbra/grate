using System.Data;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql.Infrastructure;
using grate.PostgreSql.Migration;
using Npgsql;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.TestInfrastructure;

public class PostgreSqlGrateTestContext : IGrateTestContext
{
    private readonly PostgreSqlTestContainer _testContainer;

    public PostgreSqlGrateTestContext(
        IGrateMigrator migrator,
        PostgreSqlTestContainer container)
    {
        _testContainer = container;
        Migrator = migrator;
    }

    public IGrateMigrator Migrator { get; }
    
    private string AdminPassword => _testContainer.AdminPassword;
    private int? Port => _testContainer.TestContainer.GetMappedPublicPort(_testContainer.Port);
    private string Hostname => _testContainer.TestContainer.Hostname;


    public string AdminConnectionString =>
        $"Host={Hostname};Port={Port};Database=postgres;Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";


    public string ConnectionString(string database) =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public string UserConnectionString(string database) =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public IDbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);

    public ISyntax Syntax => new PostgreSqlSyntax();
    public Type DbExceptionType => typeof(PostgresException);

    public Type DatabaseType => typeof(PostgreSqlDatabase);
    public bool SupportsTransaction => true;

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT version()",
        SleepTwoSeconds = "SELECT pg_sleep(2);"
    };

    public string ExpectedVersionPrefix => "PostgreSQL 16.";
    public bool SupportsCreateDatabase => true;

}
