using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using grate.Infrastructure;
using Xunit;

namespace Basic_tests.CommandLineParsing;

// ReSharper disable once InconsistentNaming
public class Basic_CommandLineParsing
{
    [Fact]
    public void ParserIsConfiguredCorrectly()
    {
        // Test that the parser configuration is valid, see https://github.com/dotnet/command-line-api/issues/1613
        var command = new MigrateCommand(null!);
        var configuration = new CommandLineConfiguration(command);
        configuration.ThrowIfInvalid();
    }

    [Theory]
    [InlineData("-c ")]
    [InlineData("-cs ")]
    [InlineData("--connectionstring=")]
    [InlineData("--connstring=")]
    public async Task ConnectionString(string argName)
    {
        var database = "Jajaj";
        var commandline = argName + database;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.ConnectionString.Should().Be(database);
    }

    [Theory]
    [InlineData("--accesstoken ")]
    public async Task AccessToken(string argName)
    {
        var token = "sometoken";
        var commandline = argName + token;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.AccessToken.Should().Be(token);
    }

    [Theory]
    [InlineData("-a ")]
    [InlineData("-acs ")]
    [InlineData("-csa ")]
    [InlineData("-acs=")]
    [InlineData("-csa=")]
    [InlineData("--adminconnectionstring=")]
    [InlineData("--adminconnstring=")]
    public async Task AdminConnectionString(string argName)
    {
        var database = "AdminDb";
        var commandline = argName + database;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.AdminConnectionString.Should().Be(database);
    }

    [Theory]
    [InlineData(DatabaseType.mariadb)]
    [InlineData(DatabaseType.oracle)]
    [InlineData(DatabaseType.postgresql)]
    [InlineData(DatabaseType.sqlite)]
    [InlineData(DatabaseType.sqlserver)]
    public async Task DefaultAdminConnectionString(DatabaseType databaseType)
    {
        var commandline = $"--connectionstring=;Database=jalla --databasetype={databaseType}";
        var cfg = await ParseGrateConfiguration(commandline);

        var masterDbName = GetMasterDatabaseName(databaseType);

        cfg?.AdminConnectionString.Should().Be($";Database=" + masterDbName);
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

    [Theory]
    [InlineData("-f ")]
    [InlineData("--files=")]
    [InlineData("--sqlfilesdirectory=")]
    public async Task SqlFilesDirectory(string argName)
    {
        var database = "C:\\tmp";
        var commandline = argName + database;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.SqlFilesDirectory.ToString().Should().Be(database);
    }

    [Theory]
    [InlineData("-o ")]
    [InlineData("--output ")]
    [InlineData("--output=")]
    [InlineData("--outputPath=")]
    [InlineData("--outputPath ")]
    public async Task OutputPath(string argName)
    {
        var database = "C:\\tmp";
        var commandline = argName + database;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.OutputPath.ToString().Should().Be(database);
    }

    [Theory]
    [InlineData("--version=")]
    [InlineData("--version ")]
    public async Task Version(string argName)
    {
        var version = "1.2.5.6-a";
        var commandline = argName + version;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.Version.Should().Be(version);
    }

    [Theory]
    [InlineData("-ct ")]
    [InlineData("--commandtimeout=")]
    public async Task CommandTimeout(string argName)
    {
        var timeout = 14;
        var commandline = argName + timeout;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.CommandTimeout.Should().Be(timeout);
    }

    [Theory]
    [InlineData("", false)] // by default we want token replacement
    [InlineData("--disabletokens", true)]
    [InlineData("--disabletokenreplacement", true)]
    public async Task DisableTokenReplacement(string commandline, bool expected)
    {
        var cfg = await ParseGrateConfiguration(commandline);
        cfg?.DisableTokenReplacement.Should().Be(expected);
    }

    [Theory]
    [InlineData("-cta ")]
    [InlineData("--admincommandtimeout=")]
    public async Task AdminCommandTimeout(string argName)
    {
        var timeout = 64;
        var commandline = argName + timeout;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.AdminCommandTimeout.Should().Be(timeout);
    }

    [Theory]
    [InlineData("-t")]
    [InlineData("--trx")]
    [InlineData("--transaction")]
    public async Task WithTransaction(string argName)
    {
        var commandline = argName;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.Transaction.Should().Be(true);
    }

    [Theory]
    [InlineData("-t false")]
    [InlineData("--trx false")]
    [InlineData("--transaction false")]
    [InlineData("--transaction=false")]
    public async Task WithoutTransaction(string argName)
    {
        var commandline = argName;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.Transaction.Should().Be(false);
    }

    [Theory]
    [InlineData("--env KASHMIR", "KASHMIR")]
    [InlineData("--environment JALLA", "JALLA")]
    public async Task Environment(string argName, string expected)
    {
        var commandline = argName;
        var cfg = await ParseGrateConfiguration(commandline);

        var expectedEnvironment = new GrateEnvironment(expected);

        cfg?.Environment.Should().BeEquivalentTo(expectedEnvironment);
    }

    [Theory]
    [InlineData("", "grate")]
    [InlineData("--sc RoundhousE", "RoundhousE")]
    [InlineData("--schema SquareHouse", "SquareHouse")]
    [InlineData("--schemaname TrianglehousE", "TrianglehousE")]
    public async Task Schema(string argName, string expected)
    {
        var commandline = argName;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.SchemaName.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("--silent true", true)]
    [InlineData("--silent", true)]
    [InlineData("--silent false", false)]
    [InlineData("--ni true", true)]
    [InlineData("--ni", true)]
    [InlineData("--ni false", false)]
    [InlineData("--noninteractive true", true)]
    [InlineData("--noninteractive", true)]
    [InlineData("--noninteractive false", false)]
    public async Task Silent(string argName, bool expected)
    {
        var commandline = argName;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.Silent.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("-w", true)]
    [InlineData("--warnononetimescriptchanges", true)]
    public async Task WarnOnOneTimeScriptChanges(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.WarnOnOneTimeScriptChanges.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("--donotstorescriptsruntext", true)]
    public async Task DoNotStoreScriptsRunText(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.DoNotStoreScriptsRunText.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("--runallanytimescripts", true)]
    [InlineData("--forceanytimescripts", true)]
    public async Task RunAllAnyTimeScripts(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.RunAllAnyTimeScripts.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("--dryrun", true)]
    public async Task DryRun(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.DryRun.Should().Be(expected);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("--create=false", false)]
    [InlineData("--createdatabase=false", false)]
    public async Task CreateDatabase(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.CreateDatabase.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("--warnandignoreononetimescriptchanges", true)]
    public async Task WarnAndIgnoreOnOneTimeScriptChanges(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.WarnAndIgnoreOnOneTimeScriptChanges.Should().Be(expected);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("--baseline", true)]
    public async Task Baseline(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.Baseline.Should().Be(expected);
    }

    [Fact]
    public async Task WithoutTransaction_Default()
    {
        var cfg = await ParseGrateConfiguration("");
        cfg?.Transaction.Should().Be(false);
    }


    [Theory]
    [InlineData("--silent", 0)]
    [InlineData("--ut=token=value", 1)]
    [InlineData("--ut=token=value --usertokens=abc=123", 2)]
    //[InlineData("--usertokens=token=value;abe=123", 2)] This is a back-compat scenario we may want to add support for.
    public async Task UserTokens(string args, int expectedCount)
    {
        var cfg = await ParseGrateConfiguration(args);
        var t = cfg?.UserTokens.Safe().ToList();
        t.Should().HaveCount(expectedCount);
    }


    [Theory]
    [InlineData("", DatabaseType.sqlserver)] // default
    [InlineData("--dbt=postgresql", DatabaseType.postgresql)]
    [InlineData("--dbt=mariadb", DatabaseType.mariadb)]
    [InlineData("--databasetype=mariadb", DatabaseType.mariadb)]
    [InlineData("--databasetype=MariaDB", DatabaseType.mariadb)]
    public async Task TestDatabaseType(string args, DatabaseType expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.DatabaseType.Should().Be(expected);
    }


    [Theory]
    [InlineData("", false)]
    [InlineData("--ignoredirectorynames", true)]
    [InlineData("--searchallinsteadoftraverse", true)]
    [InlineData("--searchallsubdirectoriesinsteadoftraverse", true)]
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
