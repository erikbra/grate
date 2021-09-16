using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace grate.unittests.Oracle.Running_MigrationScripts
{
    [TestFixture]
    [Category("Oracle")]
    public class Failing_Scripts
    {
        private GrateConfiguration? _config;
        private static string? AdminConnectionString() => $"Data Source=localhost,{GrateTestContext.SqlServer.Port};Initial Catalog=master;User Id=sa;Password={GrateTestContext.SqlServer.AdminPassword}";
        private static string? ConnectionString(string database) => $"Data Source=localhost,{GrateTestContext.SqlServer.Port};Initial Catalog={database};User Id=sa;Password={GrateTestContext.SqlServer.AdminPassword}";
        
        [Test]
        public async Task Aborts_the_run_giving_an_error_message()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateInvalidSql(knownFolders.Up);
            
            await using (migrator = GetMigrator(db, true, knownFolders))
            {
                var ex = Assert.ThrowsAsync<SqlException>(migrator.Migrate);
                ex?.Message.Should().Be("Incorrect syntax near 'TOP'.");
            }
        }

        [Test]
        public async Task Are_Inserted_Into_ScriptRunErrors_Table()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateInvalidSql(knownFolders.Up);
            
            await using (migrator = GetMigrator(db, true, knownFolders))
            {
                try
                {
                    await migrator.Migrate();
                }
                catch (SqlException)
                {
                }
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.ScriptsRunErrors";

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = new SqlConnection(ConnectionString(db)))
                {
                    scripts = (await conn.QueryAsync<string>(sql)).ToArray();
                }
            }

            scripts.Should().HaveCount(1);
        }
        
        [Test]
        public async Task Makes_Whole_Transaction_Rollback()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up);
            CreateInvalidSql(knownFolders.Up);
            
            await using (migrator = GetMigrator(db, true, knownFolders))
            {
                try
                {
                    await migrator.Migrate();
                }
                catch (SqlException)
                {
                }
            }

            string[] scripts;
            string sql = "SELECT text_of_script FROM grate.ScriptsRun";
            
            await using (var conn = new SqlConnection(ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().BeEmpty();
        }

        private GrateMigrator GetMigrator(string databaseName, bool createDatabase, KnownFolders knownFolders)
        {
            var connectionString = ConnectionString(databaseName);

            var dbLogger = new NullLogger<SqlServerDatabase>();
            var factory = Substitute.For<IFactory>();
            factory.GetService<DatabaseType, IDatabase>(DatabaseType.sqlserver)
                .Returns(new SqlServerDatabase(dbLogger));

            var dbMigrator = new DbMigrator(factory, new NullLogger<DbMigrator>(), new HashGenerator());
            var migrator = new GrateMigrator(new NullLogger<GrateMigrator>(), dbMigrator);

            _config = new GrateConfiguration()
            {
                CreateDatabase = createDatabase, 
                ConnectionString = connectionString,
                AdminConnectionString = AdminConnectionString(),
                Version = "a.b.c.d",
                KnownFolders = knownFolders,
                AlterDatabase = true,
                Transaction = true,
                NonInteractive = true
            };


            dbMigrator.ApplyConfig(_config);

            return migrator;
        }

        private static DirectoryInfo CreateRandomTempDirectory()
        {
            var dummyFile = Path.GetTempFileName();
            File.Delete(dummyFile);

            var scriptsDir = Directory.CreateDirectory(dummyFile);
            return scriptsDir;
        }

        private static void CreateDummySql(MigrationsFolder? folder)
        {
            var dummySql = "SELECT @@VERSION";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "1_jalla.sql", dummySql);
        }
        
        private static void CreateInvalidSql(MigrationsFolder? folder)
        {
            var dummySql = "SELECT TOP";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "2_failing.sql", dummySql);
        }

        private static void WriteSql(DirectoryInfo path, string filename, string? sql)
        {
            File.WriteAllText(Path.Combine(path.ToString(), filename), sql);
        }

        private static DirectoryInfo MakeSurePathExists(MigrationsFolder? folder)
        {
            var path = folder?.Path ?? throw new ArgumentException(nameof(folder.Path));

            if (!path.Exists)
            {
                path.Create();
            }

            return path;
        }
    }
}
