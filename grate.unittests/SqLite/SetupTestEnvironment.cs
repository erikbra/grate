using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace grate.unittests.Sqlite
{
    [SetUpFixture]
    [Category("Sqlite")]
    public class SetupTestEnvironment
    {
        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
            
            await TestContext.Progress.WriteLineAsync($"Before tests. Deleting old DB files.");
            foreach (var dbFile in dbFiles)
            {
                await TestContext.Progress.WriteLineAsync($"File: {dbFile}");
                File.Delete(dbFile);
            }
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
            
            await TestContext.Progress.WriteLineAsync($"After tests. Deleting DB files.");
            foreach (var dbFile in dbFiles)
            {
                await TestContext.Progress.WriteLineAsync($"File: {dbFile}");
                File.Delete(dbFile);
            }
        }
    }
}
