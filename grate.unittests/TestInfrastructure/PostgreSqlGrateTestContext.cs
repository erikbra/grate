using System;
using System.Data.Common;
using System.Linq;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using NSubstitute;

namespace grate.unittests.TestInfrastructure
{
    class PostgreSqlGrateTestContext : IGrateTestContext
    {
        public string AdminPassword { get; set; } = default!;
        public int? Port { get; set; }
        
        public string DockerCommand(string serverName, string adminPassword) =>
            $"run -d --name {serverName} -e POSTGRES_PASSWORD={adminPassword} -P postgres:latest";
        
        public string AdminConnectionString  => $"Host=localhost;Port={Port};Database=postgres;Username=postgres;Password={AdminPassword}";
        public string ConnectionString(string database) => $"Host=localhost;Port={Port};Database={database};Username=postgres;Password={AdminPassword}";

        public DbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);

        public ISyntax Syntax => new PostgreSqlSyntax();
        public Type DbExceptionType => typeof(PostgresException);
        
        public ILogger Logger => NullLogger();
        private static NullLogger<PostgreSqlDatabase> NullLogger() => new();

        public DatabaseType DatabaseType => DatabaseType.postgresql;
        public string DatabaseTypeName => "PostgreSQL";
        public string MasterDatabase => "postgres";

        public IDatabase DatabaseMigrator => new PostgreSqlDatabase(NullLogger());

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
                .Returns(new PostgreSqlDatabase(NullLogger()));

            var dbMigrator = new DbMigrator(factory, new NullLogger<DbMigrator>(), new HashGenerator());
            var migrator = new GrateMigrator(new NullLogger<GrateMigrator>(), dbMigrator);
            
            dbMigrator.ApplyConfig(config);
            return migrator;
        }
        
        public GrateMigrator GetMigrator(string databaseName, bool createDatabase, KnownFolders knownFolders, params string[] environments)
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
                Environments = environments.Select(env => new GrateEnvironment(env)),
                DatabaseType = DatabaseType
            };

            return GetMigrator(config);
        }
        

        public string ExpectedVersionPrefix => "PostgreSQL 13.";
    }
}
