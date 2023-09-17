using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Unit_tests.TestInfrastructure;

class SqliteGrateTestContext : TestContextBase, IGrateTestContext
{
    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }

    public string AdminConnectionString => $"Data Source=grate-sqlite.db";
    public string ConnectionString(string database) => $"Data Source={database}.db";
    public string UserConnectionString(string database) => $"Data Source={database}.db";

    public DbConnection GetDbConnection(string connectionString) => new SqliteConnection(connectionString);

    public ISyntax Syntax => new SqliteSyntax();
    public Type DbExceptionType => typeof(SqliteException);

    public DatabaseType DatabaseType => DatabaseType.sqlite;
    public bool SupportsTransaction => false;
    public string DatabaseTypeName => "Sqlite";
    public string MasterDatabase => "master";

    public IDatabase DatabaseMigrator => new SqliteDatabase(TestConfig.LogFactory.CreateLogger<SqliteDatabase>());

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT sqlite_version();",
    };


    public string ExpectedVersionPrefix => "3.32.3";
    public bool SupportsCreateDatabase => false;
}
