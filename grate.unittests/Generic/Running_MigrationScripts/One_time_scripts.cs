using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    [TestFixture]
    public abstract class One_time_scripts: MigrationsScriptsBase
    {
        [Test]
        public async Task Are_not_run_more_than_once_when_unchanged()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up);
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
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
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }
            
            WriteSomeOtherSql(knownFolders.Up);
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                Assert.ThrowsAsync<OneTimeScriptChanged>(() => migrator.Migrate());
            }

            string[] scripts;
            string sql = "SELECT text_of_script FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(1);
            scripts.First().Should().Be(Context.Sql.SelectVersion);
        }
    }
}
