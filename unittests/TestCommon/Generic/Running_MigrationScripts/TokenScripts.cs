using Dapper;
using FluentAssertions;
using grate.Configuration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Running_MigrationScripts;

public abstract class TokenScripts : MigrationsScriptsBase
{

    protected virtual string CreateDatabaseName => "create view grate as select '{{DatabaseName}}' as dbase";
    protected virtual string CreateViewMyCustomToken => "create view grate as select '{{MyCustomToken}}' as dbase";

    [Fact]
    public async Task Tokens_are_replaced()
    {
        var db = TestConfig.RandomDatabase().ToUpper();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        var path = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Views]?.Path ?? throw new Exception("Config Fail")));

        WriteSql(path, "token.sql", CreateDatabaseName);

        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        string sql = $"SELECT dbase FROM grate";
        await using var conn = Context.CreateDbConnection(db);
        var actual = await conn.QuerySingleAsync<string>(sql);
        actual.Should().Be(db);

    }

    [Fact]
    public async Task User_tokens_are_replaced()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        var path = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Views]?.Path ?? throw new Exception("Config Fail")));

        WriteSql(path, "token.sql", CreateViewMyCustomToken);

        var config = Context.GetConfiguration(db, parent, knownFolders) with
        {
            UserTokens = new[] { "mycustomtoken=token1" }, // This is important!
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
