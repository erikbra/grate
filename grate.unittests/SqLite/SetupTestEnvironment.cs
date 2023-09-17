using System.IO;
using System.Threading;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Sqlite;

[SetUpFixture]
[Category("Sqlite")]
public class SetupTestEnvironment
{
        
    static readonly ILogger<SetupTestEnvironment> Logger = TestConfig.LogFactory.CreateLogger<SetupTestEnvironment>();
        
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
            
        Logger.LogDebug($"Before tests. Deleting old DB files.");
        foreach (var dbFile in dbFiles)
        {
            TryDeletingFile(dbFile);
        }
    }

    private static void TryDeletingFile(string dbFile)
    {
        var i = 0;
        var sleepTime = 300;
        const int maxTries = 5;
        while (i++ < maxTries)
        {
            try
            {
                Logger.LogDebug("File: {DbFile}", dbFile);
                File.Delete(dbFile);
                return;
            }
            catch (IOException) when (i <= maxTries)
            {
                Thread.Sleep(sleepTime);
            }
        }
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
        SqliteConnection.ClearAllPools();
            
        var currentDirectory = Directory.GetCurrentDirectory();
        var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
            
        Logger.LogDebug("After tests. Deleting DB files.");
        foreach (var dbFile in dbFiles)
        {
            Logger.LogDebug("File: {DbFile}", dbFile);
            File.Delete(dbFile);
        }
    }
}
