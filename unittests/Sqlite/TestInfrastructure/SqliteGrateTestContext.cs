using System.Data;
using grate.Infrastructure;
using grate.Migration;
using grate.Sqlite.Infrastructure;
using grate.Sqlite.Migration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;

namespace Sqlite.TestInfrastructure;

public class SqliteGrateTestContext : IGrateTestContext
{
    public SqliteGrateTestContext(
        IGrateMigrator migrator, 
        SqliteTestContainer _)
    {
        Migrator = migrator;
    }

    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }

    public string AdminConnectionString => $"Data Source=grate-sqlite.db";
    public string ConnectionString(string database) => $"Data Source={database}.db";
    public string UserConnectionString(string database) => $"Data Source={database}.db";

    public IDbConnection GetDbConnection(string connectionString) => new SqliteConnection(connectionString);

    public ISyntax Syntax => new SqliteSyntax();
    public Type DbExceptionType => typeof(SqliteException);

    public Type DatabaseType => typeof(SqliteDatabase);
    public bool SupportsTransaction => false;
    // public string DatabaseTypeName => "Sqlite";
    // public string MasterDatabase => "master";


    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT sqlite_version();",
    };


    public string ExpectedVersionPrefix => throw new NotSupportedException("Sqlite does not support versioning");
    public bool SupportsCreateDatabase => false;

    public IGrateMigrator Migrator { get; }
}
