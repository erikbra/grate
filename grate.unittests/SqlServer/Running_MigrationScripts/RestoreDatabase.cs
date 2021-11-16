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
        
        [Test]
        public async Task Ensure_database_gets_restored()
        {
            var db = TestConfig.RandomDatabase();
            var backupFileName = "test.bak";

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Sprocs);
            
            var restoreConfig = Context.GetConfiguration(db, knownFolders) with
            {
                RestoreFromPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"SqlServer", backupFileName)
            };

            await using (var migrator = Context.GetMigrator(restoreConfig))
            {
                await migrator.Migrate();
            }

            string[] results;
            string sql = $"SELECT column1 FROM dbo.Table_1";

            await using (var conn = Context.CreateDbConnection(db))
            {
                results = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            results.First().Should().Be("testing");
        }
    }
}
