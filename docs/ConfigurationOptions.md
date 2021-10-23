# Grate Configuration Options

## Barebone
The only required argument to pass to grate is a connection string to tell it where to find your database. It will deploy to that database, looking for sql scripts in the current directory.

```
grate --connectionstring="Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=grate_test"
```

## Full Configuration

* -c, -cs, --connectionstring, --connstring <connectionstring> (REQUIRED) - You now provide an entire connection string. ServerName and Database are obsolete.
* -f, --files, --sqlfilesdirectory <sqlfilesdirectory> - The directory where your SQL scripts are [default: .]
* -o, --output, --outputPath <outputPath> - This is where everything related to the migration is stored. This includes any backups, all items that ran, permission dumps, logs, etc. [default: Users\UserName\AppData\Local\grate]
* -a, -acs, -csa, --adminconnectionstring, --adminconnstring <adminconnectionstring> - The connection string for connecting to master, if you want to create the database.  Defaults to the same as --connstring.
* -ct, --commandtimeout <commandtimeout> - This is the timeout when commands are run. This is not for admin commands or restore. [default: 60]
* -cta, --admincommandtimeout <admincommandtimeout> - This is the timeout when administration commands are run (except for restore, which has its own). [default: 300]
* --databasetype, --dbt, --dt <mariadb|oracle|postgresql|sqlite|sqlserver> - Tells grate what type of database it is running on. [default:sqlserver]
* -t, --transaction, --trx <transaction> - Run the migration in a transaction
* --env, --environment <environment> - Environment Name - This allows grate to be environment aware and only run scripts that are in a particular environment based on the name of the script.  'something.ENV.LOCAL.sql' would only be run if --env=LOCAL was set.
* --sc, --schema, --schemaname <schemaname> - The schema to use for the migration tables [default: grate]
* -ni, --noninteractive, --silent | **Silent** - tells grate not to ask for any input when it runs.
* --version <version> | **Database Version** - specify the version of the current migration directly on the command line.
* --drop | **Drop** - This instructs grate to remove the target database. Unlike RoundhousE grate will continue to run the migration scripts after the drop.
* --disabletokenreplacement, --disabletokens | **Tokens** - This instructs grate to not perform token replacement ({{somename}}). Defaults to false.
* -w, --warnononetimescriptchanges | **WarnOnOneTimeScriptChanges** - Instructs grate to execute changed one time scripts(DDL / DML in Upfolder) that have previously been run against the database instead of failing. A warning is logged for each one time script that is rerun. Defaults to false.
* --warnandignoreononetimescriptchanges | **WarnAndIgnoreOnOneTimeScriptChanges** - Instructs grate to ignore and update the hash of changed one time scripts (DDL/DML in Up folder) that have previously been run against the database instead of failing. A warning is logged for each one time scripts that is rerun. Defaults to false.
* --usertokens, --ut <usertokens> | **User Tokens** - Allows grate to perform token replacement on custom tokens ({{my_token}}). Set as a key=value pair, eg '--ut=my_token=myvalue'. Can be specified multiple times.
* --donotstorescriptsruntext | **DoNotStoreScriptsRunText** - This instructs grate to not store the full script text in the database. Defaults to false.
* --baseline | **Baseline** - This instructs grate to mark the scripts as run, but not to actually run anything against the database. Use this option if you already have scripts that have been run through other means (and BEFORE you start the new ones).
* --forceanytimescripts, --runallanytimescripts | **RunAllAnyTimeScripts** - This instructs grate to run any time scripts every time it is run even if they haven't changed. Defaults to false.
* --dryrun | **DryRun** - This instructs grate to log what would have run, but not to actually run anything against the database.  Use this option if you are trying to figure out what grate is going to do.
* -v, --verbosity <Critical|Debug|Error|Information|None|Trace|Warning> | **Verbosity level** (as defined here: https://docs.microsoft.com/dotnet/api/Microsoft.Extensions.Logging.LogLevel)
* -?, -h, --help - Show help and usage information