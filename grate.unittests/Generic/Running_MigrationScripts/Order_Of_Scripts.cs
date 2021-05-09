using System.IO;
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
    public abstract class Order_Of_Scripts: MigrationsScriptsBase
    {
        [Test()]
        public async Task Is_as_expected()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            await using (migrator = GetMigrator(db, true))
            {
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(14);

            Assert.Multiple(() =>
                {
                    scripts[0].Should().Be("1_beforemigration.sql");
                    scripts[1].Should().Be("1_alterdatabase.sql");
                    scripts[2].Should().Be("1_aftercreate.sql");
                    scripts[3].Should().Be("1_beforeup.sql");
                    scripts[4].Should().Be("1_up.sql");
                    scripts[5].Should().Be("1_firstafterup.sql");
                    scripts[6].Should().Be("1_functions.sql");
                    scripts[7].Should().Be("1_views.sql");
                    scripts[8].Should().Be("1_sprocs.sql");
                    scripts[9].Should().Be("1_triggers.sql");
                    scripts[10].Should().Be("1_indexes.sql");
                    scripts[11].Should().Be("1_afterotherany.sql");
                    scripts[12].Should().Be("1_permissions.sql");
                    scripts[13].Should().Be("1_aftermigration.sql");
                }
            );
        }


        private GrateMigrator GetMigrator(string databaseName, bool createDatabase)
        {
            var dummyFile = Path.GetTempFileName();
            File.Delete(dummyFile);

            var scriptsDir = Directory.CreateDirectory(dummyFile);

            var config = new GrateConfiguration()
            {
                CreateDatabase = createDatabase, 
                ConnectionString = Context.ConnectionString(databaseName),
                AdminConnectionString = Context.AdminConnectionString,
                Version = "a.b.c.d",
                KnownFolders = KnownFolders.In(scriptsDir),
                AlterDatabase = true,
                NonInteractive = true,
                DatabaseType = Context.DatabaseType
            };

            CreateDummySql(config.KnownFolders.AfterMigration, "1_aftermigration.sql");
            CreateDummySql(config.KnownFolders.AlterDatabase, "1_alterdatabase.sql");
            CreateDummySql(config.KnownFolders.BeforeMigration, "1_beforemigration.sql");
            CreateDummySql(config.KnownFolders.Functions, "1_functions.sql");
            CreateDummySql(config.KnownFolders.Indexes, "1_indexes.sql");
            CreateDummySql(config.KnownFolders.Permissions, "1_permissions.sql");
            CreateDummySql(config.KnownFolders.RunAfterCreateDatabase, "1_aftercreate.sql");
            CreateDummySql(config.KnownFolders.RunAfterOtherAnyTimeScripts, "1_afterotherany.sql");
            CreateDummySql(config.KnownFolders.RunBeforeUp, "1_beforeup.sql");
            CreateDummySql(config.KnownFolders.RunFirstAfterUp, "1_firstafterup.sql");
            CreateDummySql(config.KnownFolders.Sprocs, "1_sprocs.sql");
            CreateDummySql(config.KnownFolders.Triggers, "1_triggers.sql");
            CreateDummySql(config.KnownFolders.Up, "1_up.sql");
            CreateDummySql(config.KnownFolders.Views, "1_views.sql");

            return Context.GetMigrator(config);

        }
    }
}
