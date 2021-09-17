using System;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    [TestFixture]
    public abstract class TokenScripts : MigrationsScriptsBase
    {
        [Test]
        public async Task EnsureTokensAreReplaced()
        {
            var db = TestConfig.RandomDatabase();

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            var path = knownFolders?.Views?.Path ?? throw new Exception("Config Fail");
            WriteSql(path, "token.sql", "create view grate as select '{{DatabaseName}}' as dbase;");

            await using (var migrator = Context.GetMigrator(db, true, knownFolders))
            {
                await migrator.Migrate();
            }

            string sql = $"SELECT dbase FROM grate";
            await using var conn = Context.CreateDbConnection(Context.ConnectionString(db));
            var actual = await conn.QuerySingleAsync<string>(sql);
            actual.Should().Be(db);

        }
    }
}
