using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace grate.unittests.TestInfrastructure;

public interface IGrateTestContext
{
    string AdminPassword { get; set; }
    int? Port { get; set; }

    string AdminConnectionString { get; }
    string ConnectionString(string database);

    DbConnection GetDbConnection(string connectionString);

    DbConnection CreateAdminDbConnection() => GetDbConnection(AdminConnectionString);
    DbConnection CreateDbConnection(string database) => GetDbConnection(ConnectionString(database));

    ISyntax Syntax { get; }

    Type DbExceptionType { get; }
    DatabaseType DatabaseType { get; }
    bool SupportsTransaction { get; }
    IDatabase DatabaseMigrator { get; }

    SqlStatements Sql { get; }
    string DatabaseTypeName { get; }
    string MasterDatabase { get; }

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
        DatabaseType = DatabaseType
    };

    public GrateConfiguration GetConfiguration(string db, KnownFolders knownFolders) =>
        DefaultConfiguration with
        {
            ConnectionString = ConnectionString(db),
            KnownFolders = knownFolders
        };

    public GrateMigrator GetMigrator(GrateConfiguration config)
    {
        var factory = Substitute.For<IFactory>();
        factory
            .GetService<DatabaseType, IDatabase>(DatabaseType)
            .Returns(DatabaseMigrator);

        var dbMigrator = new DbMigrator(factory, TestConfig.LogFactory.CreateLogger<DbMigrator>(), new HashGenerator(), config);
        var migrator = new GrateMigrator(TestConfig.LogFactory.CreateLogger<GrateMigrator>(), dbMigrator);

        return migrator;
    }

    public GrateMigrator GetMigrator(string databaseName, KnownFolders knownFolders)
    {
        return GetMigrator(databaseName, knownFolders, null);
    }

    public GrateMigrator GetMigrator(string databaseName, KnownFolders knownFolders, string? env)
    {
        var config = DefaultConfiguration with
        {
            ConnectionString = ConnectionString(databaseName),
            KnownFolders = knownFolders,
            Environment = env != null ? new GrateEnvironment(env) : null,
        };

        return GetMigrator(config);
    }

    bool SupportsCreateDatabase { get; }
}
