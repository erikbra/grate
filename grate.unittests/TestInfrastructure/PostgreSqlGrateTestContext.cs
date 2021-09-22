using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NSubstitute;

namespace grate.unittests.TestInfrastructure
{
    class PostgreSqlGrateTestContext : TestContextBase, IGrateTestContext
    {
        public string AdminPassword { get; set; } = default!;
        public int? Port { get; set; }

        public string DockerCommand(string serverName, string adminPassword) =>
            $"run -d --name {serverName} -e POSTGRES_PASSWORD={adminPassword} -P postgres:latest";

        public string AdminConnectionString => $"Host=localhost;Port={Port};Database=postgres;Username=postgres;Password={AdminPassword};Include Error Detail=true";
        public string ConnectionString(string database) => $"Host=localhost;Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true";

        public DbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);

        public ISyntax Syntax => new PostgreSqlSyntax();
        public Type DbExceptionType => typeof(PostgresException);

        public DatabaseType DatabaseType => DatabaseType.postgresql;
        public string DatabaseTypeName => "PostgreSQL";
        public string MasterDatabase => "postgres";

        public IDatabase DatabaseMigrator => new PostgreSqlDatabase(LogFactory.CreateLogger<PostgreSqlDatabase>());

        public SqlStatements Sql => new()
        {
            SelectAllDatabases = "SELECT datname FROM pg_database",
            SelectVersion = "SELECT version()",
            SelectCurrentDatabase = "SELECT current_database()"
        };


        public GrateMigrator GetMigrator(GrateConfiguration config)
        {
            var factory = Substitute.For<IFactory>();
            factory.GetService<DatabaseType, IDatabase>(DatabaseType)
                .Returns(new PostgreSqlDatabase(LogFactory.CreateLogger<PostgreSqlDatabase>()));

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
                Transaction = true,
                Environment = env != null ? new GrateEnvironment(env) : null,
                DatabaseType = DatabaseType
            };

            return GetMigrator(config);
        }

        public string ExpectedVersionPrefix => "PostgreSQL 13.";
    }
}
