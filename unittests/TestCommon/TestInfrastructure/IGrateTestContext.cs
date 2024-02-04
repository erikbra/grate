using System.Data;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;

namespace TestCommon.TestInfrastructure;

public interface IGrateTestContext
{
    string AdminConnectionString { get; }
    string ConnectionString(string database);
    string UserConnectionString(string database);

    IDbConnection CreateAdminDbConnection() => GetDbConnection(AdminConnectionString);
    IDbConnection CreateDbConnection(string database) => GetDbConnection(ConnectionString(database));

    ISyntax Syntax { get; }

    Type DbExceptionType { get; }
    Type DatabaseType { get; }
    bool SupportsTransaction { get; }

    SqlStatements Sql { get; }
    string ExpectedVersionPrefix { get; }

    // ReSharper disable once InconsistentNaming
    public GrateConfiguration DefaultConfiguration => new()
    {
        CreateDatabase = SupportsCreateDatabase,
        AdminConnectionString = AdminConnectionString,
        Version = "a.b.c.d",
        AlterDatabase = true,
        NonInteractive = true,
        Transaction = SupportsTransaction,
    };


    public IGrateMigrator Migrator { get; }
    public bool SupportsSchemas => Migrator.SupportsSchemas();


    bool SupportsCreateDatabase { get; }
    IDbConnection GetDbConnection(string connectionString);
}
