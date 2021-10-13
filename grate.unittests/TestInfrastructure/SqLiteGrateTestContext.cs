using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace grate.unittests.TestInfrastructure
{
    class SqliteGrateTestContext : TestContextBase, IGrateTestContext
    {
        public string AdminPassword { get; set; } = default!;
        public int? Port { get; set; }

        public string AdminConnectionString => $"Data Source=grate-sqlite.db";
        public string ConnectionString(string database)  => $"Data Source={database}.db";

        public DbConnection GetDbConnection(string connectionString) => new SqliteConnection(connectionString);

        public ISyntax Syntax => new SqliteSyntax();
        public Type DbExceptionType => typeof(SqliteException);

        public DatabaseType DatabaseType => DatabaseType.sqlite;
        public string DatabaseTypeName => "Sqlite";
        public string MasterDatabase => "master";

        public IDatabase DatabaseMigrator => new SqliteDatabase(LogFactory.CreateLogger<SqliteDatabase>());

        public SqlStatements Sql => new()
        {
            SelectAllDatabases = "select name from pragma_database_list",
            SelectVersion = "SELECT sqlite_version();",
            SelectCurrentDatabase = "SELECT \"main\""
        };


        public GrateMigrator GetMigrator(GrateConfiguration config)
        {
            var factory = Substitute.For<IFactory>();
            factory.GetService<DatabaseType, IDatabase>(DatabaseType)
                .Returns(new SqliteDatabase(LogFactory.CreateLogger<SqliteDatabase>()));

            var dbMigrator = new DbMigrator(factory, LogFactory.CreateLogger<DbMigrator>(), new HashGenerator(), config);
            var migrator = new GrateMigrator(LogFactory.CreateLogger<GrateMigrator>(), dbMigrator);

            return migrator;
        }

        public GrateMigrator GetMigrator(string databaseName, bool createDatabase, KnownFolders knownFolders)
        {
            return GetMigrator(databaseName, createDatabase, knownFolders, null);
        }

        public GrateMigrator GetMigrator(string databaseName, bool createDatabase, KnownFolders knownFolders, string? env)
        {
            var config = new GrateConfiguration()
            {
                CreateDatabase = createDatabase,
                ConnectionString = ConnectionString(databaseName),
                AdminConnectionString = AdminConnectionString,
                Version = "a.b.c.d",
                KnownFolders = knownFolders,
                AlterDatabase = true,
                NonInteractive = true,
                Transaction = true,
                Environment = env != null ? new GrateEnvironment(env) : null,
                DatabaseType = DatabaseType
            };

            return GetMigrator(config);
        }


        public string ExpectedVersionPrefix => "Microsoft SQL Server 2017";
    }
}
