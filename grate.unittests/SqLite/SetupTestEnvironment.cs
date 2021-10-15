using System.IO;
using System.Threading.Tasks;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace grate.unittests.Sqlite
{
    [SetUpFixture]
    [Category("Sqlite")]
    public class SetupTestEnvironment
    {
        
        static ILogger<SetupTestEnvironment> _logger = TestConfig.LogFactory.CreateLogger<SetupTestEnvironment>();
        
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
            
            _logger.LogDebug($"Before tests. Deleting old DB files.");
            foreach (var dbFile in dbFiles)
            {
                _logger.LogDebug("File: {DbFile}", dbFile);
                File.Delete(dbFile);
            }
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dbFiles = Directory.GetFiles(currentDirectory, "*.db");
            
            _logger.LogDebug("After tests. Deleting DB files.");
            foreach (var dbFile in dbFiles)
            {
                _logger.LogDebug("File: {DbFile}", dbFile);
                File.Delete(dbFile);
            }
        }
    }
}
