using Dapper;
using FluentAssertions;
using grate.Configuration;
using SqlServer.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[Collection(nameof(SqlServerTestContainer))]
public class RestoreDatabase : SqlServerScriptsBase, IClassFixture<SimpleService>
{
    protected override IGrateTestContext Context { get; }

    protected override ITestOutputHelper TestOutput { get; }

    public RestoreDatabase(SqlServerTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqlServerGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

    private readonly string _backupPath = "/var/opt/mssql/backup/test.bak";


    private async Task RunBeforeTest()
    {
        await using var conn = Context.CreateDbConnection("master");
        await conn.ExecuteAsync("use [master] CREATE DATABASE [test]");
        await conn.ExecuteAsync("use [test] CREATE TABLE dbo.Table_1 (column1 int NULL)");
        await conn.ExecuteAsync($"BACKUP DATABASE [test] TO  DISK = '{_backupPath}'");
        await conn.ExecuteAsync("use [master] DROP DATABASE [test]");
    }

    [Fact]
    public async Task Ensure_database_gets_restored()
    {
        await RunBeforeTest();

        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        CreateDummySql(parent, knownFolders[KnownFolderKeys.Sprocs]);

        var restoreConfig = Context.GetConfiguration(db, parent, knownFolders) with
        {
            Restore = _backupPath
        };

        await using (var migrator = Context.GetMigrator(restoreConfig))
        {
            await migrator.Migrate();
        }

        int[] results;
        string sql = $"select count(1) from sys.tables where [name]='Table_1'";

        await using (var conn = Context.CreateDbConnection(db))
        {
            results = (await conn.QueryAsync<int>(sql)).ToArray();
        }

        results.First().Should().Be(1);
    }
}
