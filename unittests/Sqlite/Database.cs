using Microsoft.Data.Sqlite;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite;

[Collection(nameof(SqliteTestDatabase))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Database(SqliteGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.GenericDatabase(testContext, testOutput)
{
    
    protected override async Task CreateDatabaseFromConnectionString(string db, string connectionString)
    {
        await using var conn = new SqliteConnection(connectionString);
        conn.Open();
        await using var cmd = conn.CreateCommand();

        // Create a table to actually create the .sqlite file
        var sql = "CREATE TABLE dummy(name VARCHAR(1))";
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync();

        // Remove the table to avoid polluting the database with dummy tables :)
        sql = "DROP TABLE dummy";
        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync();
    }

    protected override async Task<IEnumerable<string>> GetDatabases()
    {
        var builder = new SqliteConnectionStringBuilder(this.Context.AdminConnectionString);
        var root = Path.GetDirectoryName(builder.DataSource) ?? Directory.CreateTempSubdirectory().ToString() ;
        var dbFiles = Directory.EnumerateFiles(root, "*.db");
        IEnumerable<string> dbNames = dbFiles
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name is not null)
            .Cast<string>();

        return await ValueTask.FromResult(dbNames);
    }

    [Fact(Skip = "SQLite does not support custom database creation script")]
    public override Task Is_created_with_custom_script_if_custom_create_database_folder_exists() =>
        Task.CompletedTask;

    [Fact(Skip = "SQLite does not support docker container")]
    public override Task Is_up_and_running_with_appropriate_database_version() => Task.CompletedTask;
    protected override bool ThrowOnMissingDatabase => false;
    
}
