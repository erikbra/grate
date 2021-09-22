using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MySqlConnector;
using NSubstitute;

namespace grate.unittests.TestInfrastructure
{
    class MariaDbGrateTestContext : TestContextBase, IGrateTestContext
    {
        public string AdminPassword { get; set; } = default!;
        public int? Port { get; set; }

        public string DockerCommand(string serverName, string adminPassword) =>
            $"run -d --name {serverName} -e MYSQL_ROOT_PASSWORD={adminPassword} -P mariadb:10.5.9";

        public string AdminConnectionString => $"Server=localhost;Port={Port};Database=mysql;Uid=root;Pwd={AdminPassword}";
        public string ConnectionString(string database) => $"Server=localhost;Port={Port};Database={database};Uid=root;Pwd={AdminPassword}";

        public DbConnection GetDbConnection(string connectionString) => new MySqlConnection(connectionString);

        public ISyntax Syntax => new MariaDbSyntax();
        public Type DbExceptionType => typeof(MySqlException);

        public DatabaseType DatabaseType => DatabaseType.mariadb;
        public string DatabaseTypeName => "MariaDB Server";
        public string MasterDatabase => "mysql";

        public IDatabase DatabaseMigrator => new MariaDbDatabase(LogFactory.CreateLogger<MariaDbDatabase>());

        public SqlStatements Sql => new()
        {
            SelectAllDatabases = "SHOW DATABASES",
            SelectVersion = "SELECT VERSION()",
            SelectCurrentDatabase = "SELECT DATABASE()"
        };


        public GrateMigrator GetMigrator(GrateConfiguration config)
        {
            var factory = Substitute.For<IFactory>();
            factory.GetService<DatabaseType, IDatabase>(DatabaseType)
                .Returns(new MariaDbDatabase(LogFactory.CreateLogger<MariaDbDatabase>()));

            var dbMigrator = new DbMigrator(factory, LogFactory.CreateLogger<DbMigrator>(), new HashGenerator());
            var migrator = new GrateMigrator(LogFactory.CreateLogger<GrateMigrator>(), dbMigrator);

            dbMigrator.ApplyConfig(config);
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
                Transaction = false,
                Environment = env != null ? new GrateEnvironment(env) : null,
                DatabaseType = DatabaseType
            };

            return GetMigrator(config);
        }


        public string ExpectedVersionPrefix => "10.5.9-MariaDB";
    }
}
