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
    public abstract class Environment_scripts : MigrationsScriptsBase
    {
        [Test]
        public async Task Are_not_run_if_not_in_environment()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up, "1_.OTHER.filename.ENV.sql");

            await using (migrator = Context.GetMigrator(db, true, knownFolders, "TEST"))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

            await using (var conn = Context.CreateDbConnection(db))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().BeEmpty();
        }

        [Test]
        public async Task Are_not_run_by_default() //Bug #101
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up, "1_.OTHER.filename.ENV.sql");

            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

            await using (var conn = Context.CreateDbConnection(db))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().BeEmpty();
        }

        [Test]
        public async Task Are_run_if_in_environment()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up, "1_.TEST.filename.ENV.sql");
            CreateDummySql(knownFolders.Up, "2_.TEST.ENV.otherfilename.sql");

            await using (migrator = Context.GetMigrator(db, true, knownFolders, "TEST"))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

            await using (var conn = Context.CreateDbConnection(db))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(2);
        }

        [Test]
        public async Task Non_environment_scripts_are_always_run()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Up, "1_.filename.sql");
            CreateDummySql(knownFolders.Up, "2_.TEST.ENV.otherfilename.sql");
            CreateDummySql(knownFolders.Up, "2_.TEST.ENV.somethingelse.sql");

            await using (migrator = Context.GetMigrator(db, true, knownFolders, "PROD"))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

            await using (var conn = Context.CreateDbConnection(db))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(1);
        }
    }
}
