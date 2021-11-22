using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.SqlServer.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServer")]
    public class RestoreDatabase : SqlServerScriptsBase
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServer;
        private readonly string _backupPath = "/var/opt/mssql/backup/test.bak";

        [OneTimeSetUp]
        public async Task RunBeforeTest()
        {
            await using (var conn = Context.CreateDbConnection("master"))
            {
                await conn.ExecuteAsync("use [master] CREATE DATABASE [test]");
                await conn.ExecuteAsync("use [test] CREATE TABLE dbo.Table_1 (column1 int NULL)");
                await conn.ExecuteAsync($"BACKUP DATABASE [test] TO  DISK = '{_backupPath}'");
                await conn.ExecuteAsync("use [master] DROP DATABASE [test]");
            }
        }
        
        [Test]
        public async Task Ensure_database_gets_restored()
        {
            var db = TestConfig.RandomDatabase();

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Sprocs);
            
            var restoreConfig = Context.GetConfiguration(db, knownFolders) with
            {
                RestoreFromPath = _backupPath
            };

            await using (var migrator = Context.GetMigrator(restoreConfig))
            {
                await migrator.Migrate();
            }

            int[] results;
            string sql = $"select count(1) from sys.tables where [name]='Table_1'";

            await using (var conn = Context.CreateDbConnection(db))
            {
                results = (await conn.QueryAsync<int>(sql)).ToArray();
            }

            results.First().Should().Be(1);
        }
    }
}
