using System.Data;
using System.Runtime.CompilerServices;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;

namespace TestCommon.TestInfrastructure;

public interface IGrateTestContext
{
    string AdminConnectionString { get; }
    string ConnectionString(string database);
    string UserConnectionString(string database);
    
    IGrateTestContext External => this;

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
        OutputPath = Directory.CreateTempSubdirectory()
    };

    public Task DropDatabase(string databaseName);


    public IGrateMigrator Migrator { get; }
    
    //public bool SupportsSchemas => Migrator.SupportsSchemas();

    bool SupportsCreateDatabase { get; }
    bool SupportsSchemas { get; }
    IDbConnection GetDbConnection(string connectionString);
}
