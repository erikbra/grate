using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using moo.Configuration;
using moo.Migration;
using static moo.Configuration.DefaultConfiguration;

namespace moo.Commands
{
    public sealed class MigrateCommand: RootCommand
    {
        //public MigrateCommand() : base("migrate", "Migrates the database")
        public MigrateCommand(IServiceProvider services) : base("Migrates the database")
        {
            Add(Database());
            Add(ConnectionString());
            Add(SqlFilesDirectory());
            Add(OutputPath());
            Add(ServerName());
            Add(ConnectionStringAdmin());
            Add(AccessToken());
            Add(CommandTimeout());
            Add(CommandTimeoutAdmin());
            Add(DatabaseType());
            Add(Version());

            Handler = CommandHandler.Create(
                async (MooConfiguration config) =>
                {
                    config.KnownFolders = KnownFolders.In(config.SqlFilesDirectory);

                    var dbMigrator = services.GetRequiredService<IDbMigrator>();
                    dbMigrator.ApplyConfig(config);
                    
                    var migrator = new MooMigrator(
                        services.GetRequiredService<ILogger<MooMigrator>>(),
                        dbMigrator
                        );
                    await migrator.Migrate();
                });
        }
        
        
        private static Option Database() =>
            new Option<string>(
                new[] {"--database", "-d", "-db", "--databasename"},
                "REQUIRED: The database you want to create/migrate") 
                {IsRequired = false};
           
        private static Option ConnectionString() =>
            new Option<string>(
                new[] {"--connectionstring", "-c", "-cs", "--connstring"},
                "REQUIRED: As an alternative to ServerName and Database - You can provide an entire connection string instead."
                )
                {IsRequired = false};
        
        private static Option ConnectionStringAdmin() =>
            new Option<string>(
                    new[] {"--connectionstringadministration", "-csa", "--connstringadmin"},
                    "This is used for connecting to master when you may have a different uid and password than normal."
                )
                {IsRequired = false};

        private static Option SqlFilesDirectory() =>
            new Option<DirectoryInfo>(
                new[] {"--sqlfilesdirectory", "-f", "--files"},
                () => new DirectoryInfo(DefaultFilesDirectory),
                "The directory where your SQL scripts are"
            ).ExistingOnly();
        
        private static Option OutputPath() =>
            new Option<DirectoryInfo>(
                new[] {"--output", "-o", "--outputPath"},
                () => new DirectoryInfo(DefaultOutputPath),
                "This is where everything related to the migration is stored. This includes any backups, all items that ran, permission dumps, logs, etc."
            ).ExistingOnly();

        private static Option ServerName() =>
            new Option<string>(
                new[] {"--instancename", "--instance", "--server", "--servername", "-s"},
                () => DefaultServerName,
                "The server and instance you would like to run on. (local) and (local)\\SQL2008 are both valid values"
            );
        
        private static Option AccessToken() =>
            new Option<string>(
                new[] {"--accesstoken"},
                "This connection property is used to connect to a SQL Database using an access token (for example Azure AD token)."
            );
        
        private static Option CommandTimeout() =>
            new Option<int>(
                new[] {"--commandtimeout", "-ct"},
                () => DefaultCommandTimeout,
                "This is the timeout when commands are run. This is not for admin commands or restore."
            );
        
        private static Option CommandTimeoutAdmin() =>
            new Option<int>(
                new[] {"--commandtimeoutadmin", "-cta"},
                () => DefaultAdminCommandTimeout,
                "This is the timeout when administration commands are run (except for restore, which has its own)."
            );
        
        private static Option DatabaseType() =>
            new Option<DatabaseType>(
                new[] {"--databasetype", "-dt", "-dbt"},
                () => Configuration.DatabaseType.sqlserver,
                "Tells moo what type of database it is running on."
            );
        
        private static Option Version() =>
            new Option<string>(
                new[] {"--version"},
                "Specify the version directly."
            );
    }
}