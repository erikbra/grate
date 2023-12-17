

using Microsoft.Data.Sqlite;
namespace TestCommon.TestInfrastructure;
public class SqliteTestContainer : IAsyncLifetime
{
    public Task DisposeAsync()
    {
        SqliteConnection.ClearAllPools();

        var currentDirectory = Directory.GetCurrentDirectory();
        var dbFiles = Directory.GetFiles(currentDirectory, "*.db");

        Console.WriteLine("After tests. Deleting DB files.");
        foreach (var dbFile in dbFiles)
        {
            //  Logger.LogDebug("File: {DbFile}", dbFile);
            File.Delete(dbFile);
        }
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var dbFiles = Directory.GetFiles(currentDirectory, "*.db");

        Console.WriteLine($"Before tests. Deleting old DB files.");
        foreach (var dbFile in dbFiles)
        {
            TryDeletingFile(dbFile);
        }
        return Task.CompletedTask;
    }
    private void TryDeletingFile(string dbFile)
    {
        var i = 0;
        var sleepTime = 300;
        const int maxTries = 5;
        while (i++ < maxTries)
        {
            try
            {
                Console.WriteLine($"File: {dbFile}");
                File.Delete(dbFile);
                return;
            }
            catch (IOException) when (i <= maxTries)
            {
                Thread.Sleep(sleepTime);
            }
        }
    }

}

[CollectionDefinition(nameof(SqliteTestContainer))]
public class SqliteTestCollection : ICollectionFixture<SqliteTestContainer>
{
}
