using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Configuration;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;
using grate.Exceptions;
using grate.Infrastructure;

#if NET6_0
using Dir = TestCommon.TestInfrastructure.Net6PolyFills.Directory;
#else
using Dir = System.IO.Directory;
#endif

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
    [InlineData("-f ")]
    [InlineData("--files=")]
    [InlineData("--sqlfilesdirectory=")]
    public async Task SqlFilesDirectory(string argName)
    {
        var folder = Dir.CreateTempSubdirectory();
        var commandline = argName + folder;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.SqlFilesDirectory.ToString().Should().Be(folder.ToString());
    }

    [Theory]
    [InlineData("-o ")]
    [InlineData("--output ")]
    [InlineData("--output=")]
    [InlineData("--outputPath=")]
    [InlineData("--outputPath ")]
    public async Task OutputPath(string argName)
    {
        var nonExistingDirectory = Path.Join(Path.GetTempPath() + Guid.NewGuid());
        var commandline = argName + nonExistingDirectory;

        CommandLineGrateConfiguration? cfg = null;
        var ex = await Record.ExceptionAsync(async () => cfg = await ParseGrateConfiguration(commandline));

        ex.Should().BeNull();
        cfg?.OutputPath.ToString().Should().Be(nonExistingDirectory);
    }

    [Theory]
    [InlineData("-r ")]
    [InlineData("-r=")]
    [InlineData("--repo ")]
    [InlineData("--repo=")]
    [InlineData("--repositorypath ")]
    [InlineData("--repositorypath=")]
    public async Task RepositoryPath(string argName)
    {
        var repositorypath = "git@example.com:user/repo.git";
        var commandline = argName + repositorypath;
        var cfg = await ParseGrateConfiguration(commandline);

        cfg?.RepositoryPath.Should().Be(repositorypath);
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
    
    
    /// <summary>
    /// We can use multiple environments, separated by space, ; or ,
    /// This makes it possible to create orhotogonal environments, and run scripts
    /// that are specific to a combination of environments.
    ///
    /// Example: You have:
    /// * Some scripts that you only run for Customer 1
    /// * Some scripts that you only run for Customer 2
    /// * some scripts that you only run for Azure
    /// * some scripts that you only run for AWS
    /// * some scripts that you only run for Dev.
    /// * some scripts that you only run for Test.
    /// * some scripts that you only run for Prod.
    ///
    /// Then, you can combine any of these environments to create a specific environment, to avoid
    /// having to create an environment for each combination.
    ///
    /// E.g.:
    /// --env Customer1;Azure
    /// --env Customer2,Azure
    /// --env Customer3
    /// --env Customer1 AWS Dev
    /// --env Customer1,AWS,Test
    /// --env Customer2;AWS;QA
    /// --env Customer2 AWS QA
    /// etc
    /// </summary>
    /// <param name="argName"></param>
    /// <param name="expected"></param>

    [Theory]
    [InlineData("--env KASHMIR", new[] {"KASHMIR"})]
    [InlineData("--env JALLA", new[] {"JALLA"})]
    [InlineData("--env JALLA KASHMIR", new[] {"JALLA", "KASHMIR"})]
    [InlineData("--env JALLA,BERGEN", new[] {"JALLA", "BERGEN"})]
    [InlineData("--env Dev;Azure;OnlyOnMondays", new[] {"Dev", "Azure", "OnlyOnMondays"})]
    [InlineData("--env Customer1;Azure;Dev", new[] {"Customer1", "Azure", "Dev"})]
    [InlineData("--env Customer1;Azure;Test", new[] {"Customer1", "Azure", "Test"})]
    [InlineData("--env Customer2;Azure;Dev", new[] {"Customer2", "Azure", "Dev"})]
    [InlineData("--env Customer2;Aws;QA", new[] {"Customer2", "Aws", "QA"})]
    [InlineData("--env Customer2;Azure;Prod", new[] {"Customer2", "Azure", "Prod"})]
    public async Task Environments(string argName, IEnumerable<string> expected)
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
    [InlineData("", DatabaseType.SQLServer)] // default
    [InlineData("--dbt=postgresql", DatabaseType.PostgreSQL)]
    [InlineData("--dbt=sqlite", DatabaseType.SQLite)]
    [InlineData("--dbt=oracle", DatabaseType.Oracle)]
    [InlineData("--dbt=mariadb", DatabaseType.MariaDB)]
    [InlineData("--databasetype=mariadb", DatabaseType.MariaDB)]
    [InlineData("--databasetype=MariaDB", DatabaseType.MariaDB)]
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
    
    [Theory]
    [InlineData("", false)]
    [InlineData("--isuptodate", true)]
    [InlineData("--isuptodate false", false)]
    [InlineData("--uptodatecheck", true)]
    [InlineData("--uptodatecheck false", false)]
    public async Task UpToDateCheck(string args, bool expected)
    {
        var cfg = await ParseGrateConfiguration(args);
        cfg?.UpToDateCheck.Should().Be(expected);
    }

    private static async Task<CommandLineGrateConfiguration?> ParseGrateConfiguration(string commandline)
    {
        // All parsing fails if the connectionstring is not supplied, so we need to add it here, if it's not in the commandline.
        if (
            (!commandline.Contains("--connectionstring=")) &&
            (!commandline.Contains("--connstring=")) &&
            (!commandline.Contains("-cs ")) &&
            (!commandline.Contains("-c ")))
        {
            commandline += " -c \"Server=.;Database=master;Trusted_Connection=True;\"";
        }

        CommandLineGrateConfiguration? cfg = null;
        var cmd = CommandHandler.Create((CommandLineGrateConfiguration config) => cfg = config);

        ParseResult p =
            new Parser(new MigrateCommand(null!)).Parse(commandline);

        if (p.Errors.Any())
        {
            var exceptions = p.Errors.Select(error => new ConfigurationErrorsException(error.Message)).ToList();
            throw new MigrationFailed(exceptions);
        }

        await cmd.InvokeAsync(new InvocationContext(p));
        return cfg;
    }
}
