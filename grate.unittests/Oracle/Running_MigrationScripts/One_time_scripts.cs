using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class One_time_scripts
    {
        private GrateConfiguration? _config;
        private static string? AdminConnectionString() => $"Data Source=localhost,{GrateTestContext.SqlServer.Port};Initial Catalog=master;User Id=sa;Password={GrateTestContext.SqlServer.AdminPassword}";
        private static string? ConnectionString(string database) => $"Data Source=localhost,{GrateTestContext.SqlServer.Port};Initial Catalog={database};User Id=sa;Password={GrateTestContext.SqlServer.AdminPassword}";

        [Test]
        public async Task Are_not_run_more_than_once_when_unchanged()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up);
            
            await using (migrator = GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }
            await using (migrator = GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.ScriptsRun";
            
            await using (var conn = new SqlConnection(ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(1);
        }
        
        [Test]
        public async Task Fails_if_changed_between_runs()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up);
            
            await using (migrator = GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }
            
            WriteSomeOtherSql(knownFolders.Up);
            
            await using (migrator = GetMigrator(db, true, knownFolders))
            {
                Assert.ThrowsAsync<OneTimeScriptChanged>(() => migrator.Migrate());
            }

            string[] scripts;
            string sql = "SELECT text_of_script FROM grate.ScriptsRun";
            
            await using (var conn = new SqlConnection(ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(1);
            scripts.First().Should().Be("SELECT @@VERSION");
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

        private void CreateDummySql(MigrationsFolder? folder)
        {
            var dummySql = "SELECT @@VERSION";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "1_jalla.sql", dummySql);
        }
        
        private static void WriteSomeOtherSql(MigrationsFolder? folder)
        {
            var dummySql = "SELECT DB_NAME()";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "1_jalla.sql", dummySql);
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
