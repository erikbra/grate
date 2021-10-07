using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using grate.Infrastructure;
using NUnit.Framework;

namespace grate.unittests
{
    [TestFixture]
    [Category("Basic")]
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
            var cfg = await ParseGrateConfiguration(commandline);

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
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.AdminConnectionString.Should().Be(database);
        }

        [TestCase("-f ")]
        [TestCase("--files=")]
        [TestCase("--sqlfilesdirectory=")]
        public async Task SqlFilesDirectory(string argName)
        {
            var database = "C:\\tmp";
            var commandline = argName + database;
            var cfg = await ParseGrateConfiguration(commandline);

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
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.OutputPath?.ToString().Should().Be(database);
        }

        [TestCase("--version=")]
        [TestCase("--version ")]
        public async Task Version(string argName)
        {
            var version = "1.2.5.6-a";
            var commandline = argName + version;
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.Version.Should().Be(version);
        }

        [TestCase("-ct ")]
        [TestCase("--commandtimeout=")]
        public async Task CommandTimeout(string argName)
        {
            var timeout = 14;
            var commandline = argName + timeout;
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.CommandTimeout.Should().Be(timeout);
        }

        [TestCase("", false)] // by default we want token replacement
        [TestCase("--disabletokens", true)]
        [TestCase("--disabletokenreplacement", true)]
        public async Task DisableTokenReplacement(string commandline, bool expected)
        {
            var cfg = await ParseGrateConfiguration(commandline);
            cfg?.DisableTokenReplacement.Should().Be(expected);
        }

        [TestCase("-cta ")]
        [TestCase("--admincommandtimeout=")]
        public async Task AdminCommandTimeout(string argName)
        {
            var timeout = 64;
            var commandline = argName + timeout;
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.AdminCommandTimeout.Should().Be(timeout);
        }

        [TestCase("-t")]
        [TestCase("--trx")]
        [TestCase("--transaction")]
        public async Task WithTransaction(string argName)
        {
            var commandline = argName;
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.Transaction.Should().Be(true);
        }

        [TestCase("-t 0")]
        [TestCase("--trx false")]
        [TestCase("--transaction false")]
        [TestCase("--transaction=false")]
        public async Task WithoutTransaction(string argName)
        {
            var commandline = argName;
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.Transaction.Should().Be(false);
        }

        [TestCase("--env KASHMIR", "KASHMIR")]
        [TestCase("--environment JALLA", "JALLA")]
        public async Task Environment(string argName, string expected)
        {
            var commandline = argName;
            var cfg = await ParseGrateConfiguration(commandline);

            var expectedEnvironment = new GrateEnvironment(expected);

            cfg?.Environment.Should().BeEquivalentTo(expectedEnvironment);
        }

        [TestCase("", "grate")]
        [TestCase("--sc RoundhousE", "RoundhousE")]
        [TestCase("--schema SquareHouse", "SquareHouse")]
        [TestCase("--schemaname TrianglehousE", "TrianglehousE")]
        public async Task Schema(string argName, string expected)
        {
            var commandline = argName;
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.SchemaName.Should().Be(expected);
        }

        [TestCase("", false)]
        [TestCase("--silent true", true)]
        [TestCase("--silent", true)]
        [TestCase("--silent false", false)]
        [TestCase("--ni true", true)]
        [TestCase("--ni", true)]
        [TestCase("--ni false", false)]
        [TestCase("--noninteractive true", true)]
        [TestCase("--noninteractive", true)]
        [TestCase("--noninteractive false", false)]
        public async Task Silent(string argName, bool expected)
        {
            var commandline = argName;
            var cfg = await ParseGrateConfiguration(commandline);

            cfg?.Silent.Should().Be(expected);
        }

        [TestCase("", false)]
        [TestCase("-w", true)]
        [TestCase("--warnononetimescriptchanges", true)]
        public async Task WarnOnOneTimeScriptChanges(string args, bool expected)
        {
            var cfg = await ParseGrateConfiguration(args);
            cfg?.WarnOnOneTimeScriptChanges.Should().Be(expected);
        }

        [TestCase("", false)]
        [TestCase("--donotstorescriptsruntext", true)]
        public async Task DoNotStoreScriptsRunText(string args, bool expected)
        {
            var cfg = await ParseGrateConfiguration(args);
            cfg?.DoNotStoreScriptsRunText.Should().Be(expected);
        }

        [TestCase("", false)]
        [TestCase("--runallanytimescripts", true)]
        [TestCase("--forceanytimescripts", true)]
        public async Task RunAllAnyTimeScripts(string args, bool expected)
        {
            var cfg = await ParseGrateConfiguration(args);
            cfg?.RunAllAnyTimeScripts.Should().Be(expected);
        }

        [TestCase("", false)]
        [TestCase("--dryrun", true)]
        public async Task DryRun(string args, bool expected)
        {
            var cfg = await ParseGrateConfiguration(args);
            cfg?.DryRun.Should().Be(expected);
        }

        [TestCase("", false)]
        [TestCase("--warnandignoreononetimescriptchanges", true)]
        public async Task WarnAndIgnoreOnOneTimeScriptChanges(string args, bool expected)
        {
            var cfg = await ParseGrateConfiguration(args);
            cfg?.WarnAndIgnoreOnOneTimeScriptChanges.Should().Be(expected);
        }

        [TestCase("", false)]
        [TestCase("--baseline", true)]
        public async Task Baseline(string args, bool expected)
        {
            var cfg = await ParseGrateConfiguration(args);
            cfg?.Baseline.Should().Be(expected);
        }

        [Test]
        public async Task WithoutTransaction_Default()
        {
            var cfg = await ParseGrateConfiguration("");
            cfg?.Transaction.Should().Be(false);
        }

        [TestCase("--silent", 0)]
        [TestCase("--ut=token=value", 1)]
        [TestCase("--ut=token=value --usertokens=abc=123", 2)]
        //[TestCase("--usertokens=token=value;abe=123", 2)] This is a back-compat scenario we may want to add support for.
        public async Task UserTokens(string args, int expectedCount)
        {
            var cfg = await ParseGrateConfiguration(args);
            var t = cfg?.UserTokens.Safe().ToList();
            t.Should().HaveCount(expectedCount);
        }

        [TestCase("", DatabaseType.sqlserver)] // default
        [TestCase("--dbt=postgresql", DatabaseType.postgresql)]
        [TestCase("--dbt=mariadb", DatabaseType.mariadb)]
        public async Task TestDatabaseType(string args, DatabaseType expected)
        {
            var cfg = await ParseGrateConfiguration(args);
            cfg?.DatabaseType.Should().Equals(expected);
        }


        private static async Task<GrateConfiguration?> ParseGrateConfiguration(string commandline)
        {
            GrateConfiguration? cfg = null;
            var cmd = CommandHandler.Create((GrateConfiguration config) => cfg = config);

            ParseResult p =
                new Parser(new MigrateCommand(null!)).Parse(commandline);
            await cmd.InvokeAsync(new InvocationContext(p));
            return cfg;
        }
    }
}
