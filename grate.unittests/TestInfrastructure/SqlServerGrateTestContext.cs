using System;
using System.Data.Common;
using System.Linq;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace grate.unittests.TestInfrastructure
{
    class SqlServerGrateTestContext : IGrateTestContext
    {
        public string AdminPassword { get; set; } = default!;
        public int? Port { get; set; }
        
        public string DockerCommand(string serverName, string adminPassword) =>
            $"run -d --name {serverName} -e ACCEPT_EULA=Y -e SA_PASSWORD={adminPassword} -e MSSQL_PID=Developer -e MSSQL_COLLATION=Danish_Norwegian_CI_AS -P mcr.microsoft.com/mssql/server:2017-latest";
        
        public string AdminConnectionString  => $"Data Source=localhost,{Port};Initial Catalog=master;User Id=sa;Password={AdminPassword}";
        public string ConnectionString(string database) => $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword}";

        public DbConnection GetDbConnection(string connectionString) => new SqlConnection(connectionString);

        public ISyntax Syntax => new SqlServerSyntax();
        public Type DbExceptionType => typeof(SqlException);
        
        public ILogger Logger => NullLogger();
        private static NullLogger<SqlServerDatabase> NullLogger() => new();

        public DatabaseType DatabaseType => DatabaseType.sqlserver;
        public string DatabaseTypeName => "SQL server";
        public string MasterDatabase => "master";

        public IDatabase DatabaseMigrator => new SqlServerDatabase(NullLogger());

        public SqlStatements Sql => new()
        {
            SelectAllDatabases = "SELECT name FROM sys.databases",
            SelectVersion = "SELECT @@VERSION",
            SelectCurrentDatabase = "SELECT DB_NAME()"
        };


        public GrateMigrator GetMigrator(GrateConfiguration config)
        {
            var factory = Substitute.For<IFactory>();
            factory.GetService<DatabaseType, IDatabase>(DatabaseType)
                .Returns(new SqlServerDatabase(NullLogger()));

            var dbMigrator = new DbMigrator(factory, new NullLogger<DbMigrator>(), new HashGenerator());
            var migrator = new GrateMigrator(new NullLogger<GrateMigrator>(), dbMigrator);
            
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
        

        public string ExpectedVersionPrefix => "Microsoft SQL Server 2017";
    }
}
