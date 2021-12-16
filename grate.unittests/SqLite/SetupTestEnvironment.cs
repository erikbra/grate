using System.IO;
using grate.unittests.TestInfrastructure;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace grate.unittests.Sqlite;

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
            Logger.LogDebug("File: {DbFile}", dbFile);
            File.Delete(dbFile);
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
