﻿using System.Data;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestCommon.TestInfrastructure;

public interface IGrateTestContext : IDatabaseConnectionFactory
{
    string AdminPassword { get; }
    int? Port { get; }

    string AdminConnectionString { get; }
    string ConnectionString(string database);
    string UserConnectionString(string database);

    //DbConnection GetDbConnection(string connectionString);

    IDbConnection CreateAdminDbConnection() => GetDbConnection(AdminConnectionString);
    IDbConnection CreateDbConnection(string database) => GetDbConnection(ConnectionString(database));

    ISyntax Syntax { get; }

    Type DbExceptionType { get; }
    string DatabaseType { get; }
    bool SupportsTransaction { get; }
    IDatabase DatabaseMigrator { get; }

    SqlStatements Sql { get; }
    //string DatabaseTypeName { get; }
    //string MasterDatabase { get; }
    IServiceProvider ServiceProvider { get; }
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

    public IGrateMigrator GetMigrator(GrateConfiguration config)
    {
        // var factory = Substitute.For<IFactory>();
        // factory
        //     .GetService<string, IDatabase>(DatabaseType)
        //     .Returns(DatabaseMigrator);
        var db = ServiceProvider.GetRequiredService<IDatabase>();
        var dbMigrator = new DbMigrator(db, ServiceProvider.GetRequiredService<ILogger<DbMigrator>>(), new HashGenerator(), config);
        var migrator = new GrateMigrator(ServiceProvider.GetRequiredService<ILogger<GrateMigrator>>(), dbMigrator);

        return migrator;
    }

    public IGrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders)
    {
        return GetMigrator(databaseName, sqlFilesDirectory, knownFolders, null, false);
    }

    public IGrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders, bool runInTransaction)
    {
        return GetMigrator(databaseName, sqlFilesDirectory, knownFolders, null, runInTransaction);
    }


    public IGrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders, string? env)
    {
        return GetMigrator(databaseName, sqlFilesDirectory, knownFolders, env, false);
    }

    public IGrateMigrator GetMigrator(string databaseName, DirectoryInfo sqlFilesDirectory, IFoldersConfiguration knownFolders, string? env, bool runInTransaction)
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
