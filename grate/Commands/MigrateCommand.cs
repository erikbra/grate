using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using static grate.Configuration.DefaultConfiguration;

namespace grate.Commands;

public sealed class MigrateCommand : RootCommand
{
    public MigrateCommand(GrateMigrator mi) : base("Migrates the database")
    {
        Add(Database());
        Add(ConnectionString());
        Add(SqlFilesDirectory());
        Add(OutputPath());
        Add(ServerName());
        Add(AdminConnectionString());
        Add(AccessToken());
        Add(CommandTimeout());
        Add(CommandTimeoutAdmin());
        Add(DatabaseType());
        Add(RunInTransaction());
        Add(Environment());
        Add(SchemaName());
        Add(Silent());
        Add(Version());
        Add(Drop());
        Add(Tokens());
        Add(WarnAndRunOnScriptChange());
        Add(WarnAndIgnoreOnScriptChange());
        Add(UserTokens());
        Add(DoNotStoreScriptText());
        Add(Baseline());
        Add(RunAllAnyTimeScripts());
        Add(DryRun());
        Add(Restore());

        Handler = CommandHandler.Create(
            async () =>
            {
                await mi.Migrate();
            });
    }

    //REQUIRED OPTIONS
    private static Option ConnectionString() =>
        new Option<string>(
                new[] { "--connectionstring", "-c", "-cs", "--connstring" },
                "You now provide an entire connection string. ServerName and Database are obsolete."
            )
            { IsRequired = true };


    //CONNECTIONSTRING OPTIONS
    private static Option AdminConnectionString() =>
        new Option<string>(
                new[] { "-csa", "-a", "--adminconnectionstring", "-acs", "--adminconnstring" },
                "The connection string for connecting to master, if you want to create the database.  Defaults to the same as --connstring."
            )
            { IsRequired = false };


    //DIRECTORY OPTIONS
    private static Option SqlFilesDirectory() =>
        new Option<DirectoryInfo>(
            new[] { "--sqlfilesdirectory", "-f", "--files" },
            () => new DirectoryInfo(DefaultFilesDirectory),
            "The directory where your SQL scripts are"
        ).ExistingOnly();

    private static Option OutputPath() =>
        new Option<DirectoryInfo>(
            new[] { "--output", "-o", "--outputPath" },
            () => new DirectoryInfo(DefaultOutputPath),
            "This is where everything related to the migration is stored. This includes any backups, all items that ran, permission dumps, logs, etc."
        ).ExistingOnly();


    //SECURITY OPTIONS
    private static Option AccessToken() =>
        new Option<string>(
            new[] { "--accesstoken" },
            "Access token to be used for logging in to SQL Server / Azure SQL Database."
        );


    //TIMEOUT OPTIONS
    private static Option CommandTimeout() =>
        new Option<int>(
            new[] { "--commandtimeout", "-ct" },
            () => DefaultCommandTimeout,
            "This is the timeout when commands are run. This is not for admin commands or restore."
        );

    private static Option CommandTimeoutAdmin() =>
        new Option<int>(
            new[] { "--admincommandtimeout", "-cta" },
            () => DefaultAdminCommandTimeout,
            "This is the timeout when administration commands are run (except for restore, which has its own)."
        );

    //DATABASE OPTIONS
    private static Option DatabaseType() =>
        new Option<DatabaseType>(
            new[] { "--databasetype", "--dt", "--dbt" },
            () => Configuration.DatabaseType.sqlserver,
            "Tells grate what type of database it is running on."
        );

    private static Option RunInTransaction() => //new Argument<bool>("-t");
        new Option<bool>(
            new[] { "--transaction", "--trx", "-t" },
            "Run the migration in a transaction"
        );

    private static Option<string> SchemaName() =>
        new(
            new[] { "--sc", "--schema", "--schemaname" },
            () => "grate",
            "The schema to use for the migration tables"
        );

    private static Option<bool> Drop() =>
        new(new[] { "--drop" },
            "Drop - This instructs grate to remove the target database.  Unlike RoundhousE grate will continue to run the migration scripts after the drop."
        );


    //ENVIRONMENT OPTIONS
    private static Option<GrateEnvironment?> Environment() =>
        new (
            aliases: new[] { "--env", "--environment" },
            parseArgument: ArgumentParsers.ParseEnvironment, // Needed in System.CommandLine beta3: https://github.com/dotnet/command-line-api/issues/1664
            description: "Environment Name - This allows grate to be environment aware and only run scripts that are in a particular environment based on the name of the script.  'something.ENV.LOCAL.sql' would only be run if --env=LOCAL was set."
        );


    //WARNING OPTIONS
    private static Option<bool> WarnAndRunOnScriptChange() =>
        new(
            new[] { "-w", "--warnononetimescriptchanges" },
            "WarnOnOneTimeScriptChanges - Instructs grate to execute changed one time scripts(DDL / DML in Up folder) that have previously been run against the database instead of failing.  A warning is logged for each one time script that is rerun. Defaults to false."
        );

    private static Option<bool> WarnAndIgnoreOnScriptChange() =>
        new(
            new[] { "--warnandignoreononetimescriptchanges" },
            "WarnAndIgnoreOnOneTimeScriptChanges - Instructs grate to ignore and update the hash of changed one time scripts (DDL/DML in Up folder) that have previously been run against the database instead of failing. A warning is logged for each one time scripts that is rerun. Defaults to false."
        );


    //TOKEN OPTIONS
    private static Option<bool> Tokens() =>
    new(
        new[] { "--disabletokenreplacement", "--disabletokens" },
        "Tokens - This instructs grate to not perform token replacement ({{somename}}). Defaults to false."
    );

    private static Option<IEnumerable<string>> UserTokens() =>
        new(
            new[] { "--ut", "--usertokens" },
            "User Tokens - Allows grate to perform token replacement on custom tokens ({{my_token}}). Set as a key=value pair, eg '--ut=my_token=myvalue'. Can be specified multiple times."
        );


    //SCRIPT OPTIONS
    private static Option<bool> DoNotStoreScriptText() =>
        new(
            new[] { "--donotstorescriptsruntext" },
            "DoNotStoreScriptsRunText - This instructs grate to not store the full script text in the database. Defaults to false."
        );

    private static Option<bool> RunAllAnyTimeScripts() =>
        new(
            new[] { "--runallanytimescripts", "--forceanytimescripts" },
            "RunAllAnyTimeScripts - This instructs grate to run any time scripts every time it is run even if they haven't changed. Defaults to false."
        );


    //MISC OPTIONS
    private static Option<bool> Baseline() =>
        new(
            new[] { "--baseline" },
            "Baseline - This instructs grate to mark the scripts as run, but not to actually run anything against the database. Use this option if you already have scripts that have been run through other means (and BEFORE you start the new ones)."
        );
            
    private static Option<bool> DryRun() =>
        new(
            new[] { "--dryrun" },
            " DryRun - This instructs grate to log what would have run, but not to actually run anything against the database.  Use this option if you are trying to figure out what grate is going to do."
        );

    private static Option<string> Restore() =>
        new(
            new[] { "--restore" },
            " Restore - This instructs grate where to get the backed up database file. Defaults to NULL."
        );

    private static Option<bool> Silent() =>
    new(
        new[] { "--noninteractive", "-ni", "--ni", "--silent" },
        "Silent - tells grate not to ask for any input when it runs."
    );

    private static Option<string> Version() =>
        new(
            new[] { "--version" },
            "Database Version - specify the version of the current migration directly on the command line."
        );


    //OBSOLETE OPTIONS
    private static Option Database() =>
        new Option<string>(
                new[] { "--database" },
                "OBSOLETE: Please specify the connection string instead")
        { IsRequired = false };

    private static Option ServerName() =>
        new Option<string>(
            new[] { "--instance", "--server", "--servername", "-s" },
            //() => DefaultServerName,
            "OBSOLETE: Please specify the connection string instead."
        );
}
