

using Microsoft.Data.Sqlite;
using Xunit.Sdk;
namespace TestCommon.TestInfrastructure;
public class SqliteTestDatabase(IMessageSink messageSink) : ITestDatabase
{
    public Task DisposeAsync()
    {
        SqliteConnection.ClearAllPools();

        var currentDirectory = Directory.GetCurrentDirectory();
        var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
        var message = new DiagnosticMessage("After tests. Deleting DB files.");
        messageSink.OnMessage(message);
        foreach (var dbFile in dbFiles)
        {
            var deleteMessage = new DiagnosticMessage("File: {0}", dbFile);
            messageSink.OnMessage(deleteMessage);
            File.Delete(dbFile);
        }
        return Task.CompletedTask;
    }

    public Task InitializeAsync()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
        var message = new DiagnosticMessage("Before tests. Deleting old DB files.");
        messageSink.OnMessage(message);
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
                var message = new DiagnosticMessage("File: {0}", dbFile);
                messageSink.OnMessage(message);
                File.Delete(dbFile);
                return;
            }
            catch (IOException) when (i <= maxTries)
            {
                Thread.Sleep(sleepTime);
            }
        }
    }
    
    public string AdminConnectionString => $"Data Source=grate-sqlite.db";
    public string ConnectionString(string database) => $"Data Source={database}.db";
    public string UserConnectionString(string database) => $"Data Source={database}.db";
}
