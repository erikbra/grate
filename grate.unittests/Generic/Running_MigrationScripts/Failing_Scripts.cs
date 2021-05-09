using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    [TestFixture]
    public abstract class Failing_Scripts: MigrationsScriptsBase
    {
        protected abstract string ExpextedErrorMessageForInvalidSql { get; }
        
        [Test]
        public async Task Aborts_the_run_giving_an_error_message()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateInvalidSql(knownFolders.Up);
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                var ex = Assert.ThrowsAsync(Context.DbExceptionType, migrator.Migrate);
                ex?.Message.Should().Be(ExpextedErrorMessageForInvalidSql);
            }
        }

        [Test]
        public async Task Are_Inserted_Into_ScriptRunErrors_Table()
        {
            var db = TestConfig.RandomDatabase();

            GrateMigrator? migrator;
            
            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            CreateInvalidSql(knownFolders.Up);
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                try
                {
                    await migrator.Migrate();
                }
                catch (DbException)
                {
                }
            }

            string[] scripts;
            string sql = "SELECT script_name FROM grate.\"ScriptsRunErrors\"";

            using (new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
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
            
            await using (migrator = Context.GetMigrator(db, true, knownFolders))
            {
                try
                {
                    await migrator.Migrate();
                }
                catch (DbException)
                {
                }
            }

            string[] scripts;
            string sql = "SELECT text_of_script FROM grate.\"ScriptsRun\"";
            
            await using (var conn = Context.CreateDbConnection(Context.ConnectionString(db)))
            {
                scripts = (await conn.QueryAsync<string>(sql)).ToArray();
            }

            scripts.Should().BeEmpty();
        }
        
        private static void CreateInvalidSql(MigrationsFolder? folder)
        {
            var dummySql = "SELECT TOP";
            var path = MakeSurePathExists(folder);
            WriteSql(path, "2_failing.sql", dummySql);
        }
    }
}
