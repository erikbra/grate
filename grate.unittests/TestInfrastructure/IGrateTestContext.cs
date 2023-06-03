using System;
using System.Data.Common;
using System.IO;
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
    string UserConnectionString(string database);

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

    public GrateConfiguration GetConfiguration(string db, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders) =>
        DefaultConfiguration with
        {
            ConnectionString = ConnectionString(db),
            Folders = knownFolders,
            SqlFilesDirectory = sqlFilesDirectory
        };

    public GrateConfiguration GetConfiguration(string databaseName, DirectoryInfo sqlFilesDirectory,
        IFoldersConfiguration knownFolders, string? env, bool runInTransaction) =>
        DefaultConfiguration with
        {
            ConnectionString = ConnectionString(databaseName),
            Folders = knownFolders,
            Environment = env != null ? new GrateEnvironment(env) : null,
            Transaction = runInTransaction,
            SqlFilesDirectory = sqlFilesDirectory
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

    public GrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders)
    {
        return GetMigrator(databaseName, sqlFilesDirectory, knownFolders, null, false);
    }
    
    public GrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders, bool runInTransaction)
    {
        return GetMigrator(databaseName, sqlFilesDirectory, knownFolders, null, runInTransaction);
    }
    
    
    public GrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders, string? env)
    {
        return GetMigrator(databaseName, sqlFilesDirectory, knownFolders, env, false);
    }

    public GrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders, string? env, bool runInTransaction)
    {
        var config = DefaultConfiguration with
        {
            ConnectionString = ConnectionString(databaseName),
            Folders = knownFolders,
            Environment = env != null ? new GrateEnvironment(env) : null,
            Transaction = runInTransaction,
            SqlFilesDirectory = sqlFilesDirectory
        };

        return GetMigrator(config);
    }

    bool SupportsCreateDatabase { get; }
}
