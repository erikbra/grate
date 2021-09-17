using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static grate.Configuration.DefaultConfiguration;

namespace grate.Commands
{
    public sealed class MigrateCommand : RootCommand
    {
        //public MigrateCommand() : base("migrate", "Migrates the database")
        public MigrateCommand(IServiceProvider services) : base("Migrates the database")
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
            Add(Tokens());

            Handler = CommandHandler.Create(
                async (GrateConfiguration config) =>
                {
                    config.KnownFolders = KnownFolders.In(config.SqlFilesDirectory);

                    var dbMigrator = services.GetRequiredService<IDbMigrator>();
                    dbMigrator.ApplyConfig(config);

                    var migrator = new GrateMigrator(
                        services.GetRequiredService<ILogger<GrateMigrator>>(),
                        dbMigrator
                        );
                    await migrator.Migrate();
                });
        }


        private static Option Database() =>
            new Option<string>(
                new[] { "--database" },
                "OBSOLETE: Please specify the connection string instead")
            { IsRequired = false };

        private static Option ConnectionString() =>
            new Option<string>(
                new[] { "--connectionstring", "-c", "-cs", "--connstring" },
                "You now provide an entire connection string. ServerName and Database are obsolete."
                )
            { IsRequired = true };

        private static Option AdminConnectionString() =>
            new Option<string>(
                new[] { "-csa", "-a", "--adminconnectionstring", "-acs", "--adminconnstring" },
                    "The connection string for connecting to master, if you want to create the database."
                )
            { IsRequired = false };

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

        private static Option ServerName() =>
            new Option<string>(
                new[] { "--instance", "--server", "--servername", "-s" },
                //() => DefaultServerName,
                "OBSOLETE: Please specify the connection string instead."
            );

        private static Option AccessToken() =>
            new Option<string>(
                new[] { "--accesstoken" },
                "OBSOLETE: Please specify the connection string instead."
            );

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

        private static Option DatabaseType() =>
            new Option<DatabaseType>(
                new[] { "--databasetype", "--dt", "--dbt" },
                () => Configuration.DatabaseType.sqlserver,
                "Tells grate what type of database it is running on."
            );

        private static Option RunInTransaction() => //new Argument<bool>("-t");
            new Option<string>(
                new[] { "--transaction", "--trx", "-t" },
                "Run the migration in a transaction"
                );

        private static Option<GrateEnvironment> Environment() =>
            new(
                new[] { "--env", "--environment" }, // we'll only support a single environment initially
                "Environment Name - This allows grate to be environment aware and only run scripts that are in a particular environment based on the name of the script.  'something.ENV.LOCAL.sql' would only be run if --env=LOCAL was set."
            );

        private static Option<string> SchemaName() =>
            new(
                new[] { "--sc", "--schema", "--schemaname" },
                () => "grate",
                "The schema to use for the migration tables"
            );

        private static Option<bool> Silent() =>
            new(
                new[] { "--noninteractive", "-ni", "--ni", "--silent" },
                "Silent - tells grate not to ask for any input when it runs."
            );

        private static Option<string> Version() =>
            new(
                new[] { "--version" }, // we can't use --version as it conflicts with the standard option
                "Database Version - specify the version of the current migration directly on the command line."
            );/*
            { Name = "version" }; // But still bind to the `Version` property, this also allows using `grate version=1.1.1.1` if wanted */

        private static Option<bool> Tokens() =>
            new(
                new[] { "--disabletokenreplacement", "--disabletokens" },
                "Tokens - This instructs grate to not perform token replacement ({{somename}}). Defaults to false."
            );
    }
}
