using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    public abstract class DropDatabase : MigrationsScriptsBase
    {
        [Test]
        public async Task Ensure_database_gets_dropped()
        {
            var db = TestConfig.RandomDatabase();

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateDummySql(knownFolders.Sprocs);

            var dropConfig = new GrateConfiguration()
            {
                Drop = true, // This is important!

                CreateDatabase = true,
                ConnectionString = Context.ConnectionString(db),
                AdminConnectionString = Context.AdminConnectionString,
                Version = "a.b.c.d",
                KnownFolders = knownFolders,
                AlterDatabase = true,
                NonInteractive = true,
                Transaction = true,
                DatabaseType = Context.DatabaseType
            };

            await using (var migrator = Context.GetMigrator(dropConfig))
            {
                await migrator.Migrate();
            }

            WriteSomeOtherSql(knownFolders.Sprocs);

            await using (var migrator = Context.GetMigrator(dropConfig))
            {
                // This second migration should drop and recreate, so only one script run afterwards
                await migrator.Migrate();
            }

            string[] scripts;
            string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().HaveCount(1); // only one script because the databse was dropped after the first migration...


        }
    }
}
