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
            await using var conn = Context.CreateDbConnection(db);
            var actual = await conn.QuerySingleAsync<string>(sql);
            actual.Should().Be(db);

        }

        [Test]
        public async Task EnsureUserTokensAreReplaced()
        {
            var db = TestConfig.RandomDatabase();

            var knownFolders = KnownFolders.In(CreateRandomTempDirectory());
            var path = knownFolders?.Views?.Path ?? throw new Exception("Config Fail");
            WriteSql(path, "token.sql", "create view grate as select '{{MyCustomToken}}' as dbase;");

            var config = new GrateConfiguration()
            {
                UserTokens = new[] {"mycustomtoken=token1"}, // This is important!

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

            await using (var migrator = Context.GetMigrator(config))
            {
                await migrator.Migrate();
            }

            string sql = $"SELECT dbase FROM grate";
            await using var conn = Context.CreateDbConnection(db);
            var actual = await conn.QuerySingleAsync<string>(sql);
            actual.Should().Be("token1");

        }
    }
}
