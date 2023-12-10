﻿using Microsoft.Data.Sqlite;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite;

[Collection(nameof(SqliteTestContainer))]
public class Database : TestCommon.Generic.GenericDatabase, IClassFixture<SimpleService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public Database(SqliteTestContainer testContainer, SimpleService simpleService, ITestOutputHelper testOutput)
    {
        Context = new SqliteGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }


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
        var dbFiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.db");
        IEnumerable<string> dbNames = dbFiles
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name is not null)
            .Cast<string>();

        return await ValueTask.FromResult(dbNames);
    }

    [Fact(Skip = "SQLite does not support custom database creation script")]
    public override Task Is_created_with_custom_script_if_custom_create_database_folder_exists() =>
        Task.CompletedTask;

    protected override bool ThrowOnMissingDatabase => false;
}
