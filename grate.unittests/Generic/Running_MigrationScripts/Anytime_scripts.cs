using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    [TestFixture]
    public abstract class Anytime_scripts : MigrationsScriptsBase
    {
        [Test]
        public async Task Are_not_run_more_than_once_when_unchanged()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Sprocs);
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(1);
        }
        
        [Test]
        public async Task Are_run_again_if_changed_between_runs()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Sprocs);
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }
            
            WriteSomeOtherSql(knownFolders.Sprocs);
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }
            
            string[] scripts;
            string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(2);

            using (new AssertionScope())
            {
                scripts.First().Should().Be(Context.Sql.SelectVersion);
                scripts.Last().Should().Be(Context.Sql.SelectCurrentDatabase);
            }
        }
    }
}
