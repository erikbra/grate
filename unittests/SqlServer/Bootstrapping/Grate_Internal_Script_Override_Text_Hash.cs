using SqlServer.TestInfrastructure;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using Dapper;
using static grate.Configuration.KnownFolderKeys;
using grate.Configuration;
using grate.SqlServer.Migration;
namespace SqlServer.Bootstrapping;

[Collection(nameof(SqlServerGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Grate_Internal_Script_Override_Text_Hash(SqlServerGrateTestContext testContext, ITestOutputHelper testOutput) : MigrationsScriptsBase(testContext, testOutput)
{
    [Theory]
    [Trait("Bug", "591")]
    [InlineData("grate-internal/02_create_scripts_run_table.sql", "gE8OY/EW3vF9XZpo4I+Rcw==")]
    [InlineData("02_create_scripts_run_table.sql", "/oMcz/9sAOdvsGPc4BrPRg==")]
    [InlineData("grate-internal/03_create_scripts_run_errors_table.sql", "jRz3vUCeAhoeR+ui+bvCOw==")]
    [InlineData("03_create_scripts_run_errors_table.sql", "AD4FAu7j5tI2uYLuRQPQ7g==")]
    public async Task Should_run_the_migration_script_success_with_legacy_hash(string scriptName, string originalHash)
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        CreateDummySql(parent, knownFolders[Sprocs]);
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();
        await using var migrator = Context.Migrator.WithConfiguration(config);
        await migrator.Migrate();
        string sql = $"""
                        UPDATE {Context.Syntax.TableWithSchema("grate", "GrateScriptsRun")}
                        SET text_hash = @NewHash
                        WHERE script_name = @ScriptName;
                    """;
        using var conn = Context.External.CreateDbConnection(db);
        await conn.ExecuteAsync(sql, new { NewHash = originalHash, ScriptName = scriptName });
        await using var migrator1 = Context.Migrator.WithConfiguration(config);
        await migrator1.Migrate();
    }

    [Theory]
    [Trait("Bug", "591")]
    [InlineData("grate-internal/02_create_scripts_run_table.sql", "gE8OY/EW3vF9XZpo4I+Rcw==", "ZAsLCwIdo/5MvLxhKFyF2w==")]
    [InlineData("02_create_scripts_run_table.sql", "/oMcz/9sAOdvsGPc4BrPRg==", "IJ90PTu7hnjspwFTktXmOQ==")]
    [InlineData("grate-internal/03_create_scripts_run_errors_table.sql", "jRz3vUCeAhoeR+ui+bvCOw==", "+D1LJRaVcXf8+DW9gSfj4g==")]
    [InlineData("03_create_scripts_run_errors_table.sql", "AD4FAu7j5tI2uYLuRQPQ7g==", "27wFX+pBxrD84m5OK+zaYg==")]
    [InlineData("grate-internal/02_create_scripts_run_table.sql", "random-hash-no-override", "random-hash-no-override")]
    [InlineData("02_create_scripts_run_table.sql", "random-hash-no-override", "random-hash-no-override")]
    [InlineData("grate-internal/03_create_scripts_run_errors_table.sql", "random-hash-no-override", "random-hash-no-override")]
    [InlineData("03_create_scripts_run_errors_table.sql", "random-hash-no-override", "random-hash-no-override")]
    public void GetHashOverrideIfNeeded_returns_override_when_exists(string scriptName, string scriptHashText, string expected)
    {
        var db = new SqlServerDatabase(default!);
        var result = InvokeGetHashOverrideIfNeeded(db, scriptName, scriptHashText);
        Assert.Equal(expected, result);
    }
    private static string? InvokeGetHashOverrideIfNeeded(SqlServerDatabase db, string scriptName, string scriptHashText)
    {
        var method = typeof(SqlServerDatabase).GetMethod("GetHashOverrideIfNeeded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (string?)method?.Invoke(db, [scriptName, scriptHashText]);
    }
}
