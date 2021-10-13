using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using grate.unittests.TestInfrastructure;
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
            Trace.Listeners.Add(new ConsoleTraceListener());
            
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
