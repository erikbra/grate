using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using grate.Infrastructure;
using TestCommon.TestInfrastructure;
using NUnit.Framework;

namespace Basic_tests.CommandLineParsing;

[TestFixture]
[Category("Basic tests")]
// ReSharper disable once InconsistentNaming
public class Basic_CommandLineParsing
{
    [TestCase]
    public void ParserIsConfiguredCorrectly()
    {
        // Test that the parser configuration is valid, see https://github.com/dotnet/command-line-api/issues/1613
        var command = new MigrateCommand(null!);
        var configuration = new CommandLineConfiguration(command);
        configuration.ThrowIfInvalid();
    }

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

    [TestCase("--accesstoken ")]
    public async Task AccessToken(string argName)
    {
        var token = "sometoken";
        var commandline = argName + token;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.AccessToken.Should().Be(token);
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

    [TestCase(DatabaseType.mariadb)]
    [TestCase(DatabaseType.oracle)]
    [TestCase(DatabaseType.postgresql)]
    [TestCase(DatabaseType.sqlite)]
    [TestCase(DatabaseType.sqlserver)]
    public async Task DefaultAdminConnectionString(DatabaseType databaseType)
    {
        var commandline = $"--connectionstring=;Database=jalla --databasetype={databaseType}";
        var cfg = await ParseGrateConfiguration(commandline);

        var masterDbName = GetMasterDatabaseName(databaseType);

        cfg?.AdminConnectionString.Should().Be($";Database="+masterDbName);
    }

    private string GetMasterDatabaseName(DatabaseType databaseType) => databaseType switch
    {
        DatabaseType.mariadb => "mysql",
        DatabaseType.oracle => "oracle",
        DatabaseType.postgresql => "postgres",
        DatabaseType.sqlite => "master",
        DatabaseType.sqlserver => "master",
        _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, "Invalid database type: " + databaseType)
    };

    [TestCase("-f ")]
    [TestCase("--files=")]
    [TestCase("--sqlfilesdirectory=")]
    public async Task SqlFilesDirectory(string argName)
    {
        var database = "C:\\tmp";
        var commandline = argName + database;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.SqlFilesDirectory.ToString().Should().Be(database);
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

        cfg?.OutputPath.ToString().Should().Be(database);
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

    [TestCase("-t false")]
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

    [TestCase("", true)]
    [TestCase("--create=false", false)]
    [TestCase("--createdatabase=false", false)]
    public async Task CreateDatabase(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.CreateDatabase.Should().Be(expected);
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
        cfg?.DatabaseType.Should().Be(expected);
    }

    [TestCase("", false)]
    [TestCase("--ignoredirectorynames", true)]
    [TestCase("--searchallinsteadoftraverse", true)]
    [TestCase("--searchallsubdirectoriesinsteadoftraverse", true)]
    public async Task IgnoreDirectoryNames(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.IgnoreDirectoryNames.Should().Be(expected);
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
