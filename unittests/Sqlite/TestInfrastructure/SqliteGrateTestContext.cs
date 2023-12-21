using System.Data.Common;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;

namespace Sqlite.TestInfrastructure;

public class SqliteGrateTestContext : IGrateTestContext
{
    public SqliteGrateTestContext(IServiceProvider serviceProvider, SqliteTestContainer testContainer)
    {
        ServiceProvider = serviceProvider;
        Syntax = ServiceProvider.GetService<ISyntax>()!;
        DatabaseMigrator = ServiceProvider.GetService<IDatabase>()!;
    }
    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }

    public string AdminConnectionString => $"Data Source=grate-sqlite.db";
    public string ConnectionString(string database) => $"Data Source={database}.db";
    public string UserConnectionString(string database) => $"Data Source={database}.db";

    public DbConnection GetDbConnection(string connectionString) => new SqliteConnection(connectionString);

    //public ISyntax Syntax => new SqliteSyntax();
    public ISyntax Syntax { get; init; }
    public Type DbExceptionType => typeof(SqliteException);

    public string DatabaseType => "sqlite";
    public bool SupportsTransaction => false;
    public string DatabaseTypeName => "Sqlite";
    public string MasterDatabase => "master";

    // public IDatabase DatabaseMigrator => new SqliteDatabase(ServiceProvider.GetRequiredService<ILogger<SqliteDatabase>>());
    public IDatabase DatabaseMigrator { get; init; }

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT sqlite_version();",
    };


    public string ExpectedVersionPrefix => "3.32.3";
    public bool SupportsCreateDatabase => false;

    public IServiceProvider ServiceProvider { get; }
}
