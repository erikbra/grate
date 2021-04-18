using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using moo.Commands;
using moo.Configuration;
using moo.Infrastructure;
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
        
        [TestCase("-t")]
        [TestCase("--trx")]
        [TestCase("--transaction")]
        public async Task WithTransaction(string argName)
        {
            var commandline = argName;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.Transaction.Should().Be(true);
        }
        
        [TestCase("-t 0")]
        [TestCase("--trx false")]
        [TestCase("--transaction false")]
        [TestCase("--transaction=false")]
        public async Task WithoutTransaction(string argName)
        {
            var commandline = argName;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.Transaction.Should().Be(false);
        }
        
        [TestCase("--env KASHMIR", "KASHMIR")]
        [TestCase("--environment JALLA", "JALLA")]
        [TestCase("--environments JALLA NALLA", "JALLA", "NALLA")]
        [TestCase("--environments JALLA NALLA OTHER --trx", "JALLA", "NALLA", "OTHER")]
        public async Task Environments(string argName, params string[] expected)
        {
            var commandline = argName;
            var cfg = await ParseMooConfiguration(commandline);

            cfg?.Environments.Should().BeEquivalentTo(expected.Select(e => new MooEnvironment(e)));
        }
        
        [Test]
        public async Task WithoutTransaction_Default()
        {
            var cfg = await ParseMooConfiguration("");
            cfg?.Transaction.Should().Be(false);
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