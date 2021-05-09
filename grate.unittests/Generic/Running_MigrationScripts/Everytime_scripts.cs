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
    public abstract class Everytime_scripts : MigrationsScriptsBase
    {
        [Test]
        public async Task Are_run_every_time_even_when_unchanged()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Permissions);

            for (var i = 0; i < 3; i++)
            {
                await using (migrator = Context.GetMigrator(db, true, knownFolders))
                {
                    await migrator.Migrate();
                }
            }

            string[] scripts;
            string sql = $"SELECT text_of_script FROM grate.{Context.Syntax.Quote("ScriptsRun")}";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(3);
        }
        
        [Test]
        public async Task Are_recognized_by_script_name()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());

            var folder = knownFolders.Up;// not an everytime folder
            
            CreateDummySql(folder); 
            CreateEveryTimeScriptFile(folder); 
            CreateOtherEveryTimeScriptFile(folder); 

            for (var i = 0; i < 3; i++)
            {
                await using (migrator = Context.GetMigrator(db, true, knownFolders))
                {
                    await migrator.Migrate();
                }
            }

            string[] scripts;
            string sql = $"SELECT text_of_script FROM grate.{Context.Syntax.Quote("ScriptsRun")}";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(7); // one time script ran once, the two everytime scripts ran every time.
        }

        private void CreateEveryTimeScriptFile(MigrationsFolder? folder)
        {
            var dummySql = Context.Sql.SelectCurrentDatabase;
            var path = MakeSurePathExists(folder);
            WriteSql(path, "everytime.1_jalla.sql", dummySql);
        }
        
        private void CreateOtherEveryTimeScriptFile(MigrationsFolder? folder)
        {
            var dummySql = Context.Sql.SelectCurrentDatabase;
            var path = MakeSurePathExists(folder);
            WriteSql(path, "1_jalla.everytime.and.always.sql", dummySql);
        }

    }
}
