using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using moo.Commands;
using moo.Configuration;
using NUnit.Framework;

namespace moo.unittests
{
    [TestFixture]
    public class CommandLineParsing
    {
        [TestCase("-c ")]
        [TestCase("-cs ")]
        [TestCase("--connectionstring=")]
        [TestCase("--connstring=")]
        public async Task ConnectionString(string argName)
        {
            var database = "Jajaj";
            var commandline = argName + database;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.ConnectionString.Should().Be(database);
        }
        
        [TestCase("-a ")]
        [TestCase("-acs ")]
        [TestCase("-csa ")]
        [TestCase("-acs=")]
        [TestCase("-csa=")]
        [TestCase("--adminconnectionstring=")]
        [TestCase("--adminconnstring=")]
        public async Task AdminConnectionString(string argName)
        {
            var database = "AdminDb";
            var commandline = argName + database;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.AdminConnectionString.Should().Be(database);
        }
        
        [TestCase("-f ")]
        [TestCase("--files=")]
        [TestCase("--sqlfilesdirectory=")]
        public async Task SqlFilesDirectory(string argName)
        {
            var database = "C:\\tmp";
            var commandline = argName + database;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.SqlFilesDirectory?.ToString().Should().Be(database);
        }
        
        [TestCase("-o ")]
        [TestCase("--output ")]
        [TestCase("--output=")]
        [TestCase("--outputPath=")]
        [TestCase("--outputPath ")]
        public async Task OutputPath(string argName)
        {
            var database = "C:\\tmp";
            var commandline = argName + database;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.OutputPath?.ToString().Should().Be(database);
        }
        
        [TestCase("--version=")]
        [TestCase("--version ")]
        public async Task Version(string argName)
        {
            var version = "1.2.5.6-a";
            var commandline = argName + version;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.Version.Should().Be(version);
        }
        
        [TestCase("-ct ")]
        [TestCase("--commandtimeout=")]
        public async Task CommandTimeout(string argName)
        {
            var timeout = 14;
            var commandline = argName + timeout;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.CommandTimeout.Should().Be(timeout);
        }
        
        [TestCase("-cta ")]
        [TestCase("--admincommandtimeout=")]
        public async Task AdminCommandTimeout(string argName)
        {
            var timeout = 64;
            var commandline = argName + timeout;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.AdminCommandTimeout.Should().Be(timeout);
        }
        

        private static async Task<MooConfiguration?> ParseMooConfiguration(string commandline)
        {
            MooConfiguration? cfg = null;
            var cmd = CommandHandler.Create((MooConfiguration config) => cfg = config);

            ParseResult p =
                new Parser(new MigrateCommand(new ServiceCollection().BuildServiceProvider())).Parse(commandline);
            await cmd.InvokeAsync(new InvocationContext(p));
            return cfg;
        }
    }
}