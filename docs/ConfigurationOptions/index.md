---
layout: default
title: Configuration options
permalink: /configuration-options/
nav_order: 5
has_children: true
---

# Grate Configuration Options

## Barebone
The only required argument to pass to grate is a connection string to tell it where to find your database. It will deploy to that database, looking for sql scripts in the current directory.

```
grate --connectionstring="Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=grate_test"
```

## Full Configuration


| Option | Default | Purpose |
| ------ | ------- | ------- |
| -c<br>-cs<br>--connectionstring<br>--connstring &lt;connectionstring&gt; | - | **REQUIRED** You now provide an entire connection string. ServerName and Database are obsolete. |
| -a<br>-acs<br>-csa<br>--adminconnectionstring<br>--adminconnstring &lt;adminconnectionstring&gt; | Same as --connectionstring | The connection string for connecting to master, if you want to create the database. |
| -f<br>--files<br>--sqlfilesdirectory &lt;sqlfilesdirectory&gt; | . (current directory) | The directory where your SQL scripts are located |
| -o<br>--output<br>--outputPath &lt;outputPath&gt; | %LOCALAPPDATA%/grate | This is where everything related to the migration is stored. This includes any backups, all items that ran, permission dumps, logs, etc. |
| --accesstoken &lt;token&gt; | - | Specify an access token to use when connecting to SQL Server. |
| -ct<br>--commandtimeout &lt;commandtimeout&gt; | 60s | This is the timeout when commands are run. This is not for admin commands or restore. |
| -cta<br>--admincommandtimeout &lt;admincommandtimeout&gt; | 300 | This is the timeout when administration commands are run (except for restore, which has its own) |
| --databasetype<br>--dbt<br>--dt <mariadb \| oracle \| postgresql \| sqlite \| sqlserver> | sqlserver | Tells grate what type of database it is running on. |
| -t<br>--transaction<br>--trx <transaction> | true | Run the migration in a transaction |
| --sc<br>--schema<br>--schemaname &lt;schemaname&gt; | grate | The schema to use for the migration tables.  If you're upgrading from RoundhousE you'll probably want this! |
| --drop | false | **Drop** - This instructs grate to remove the target database. Unlike RoundhousE grate will continue to run the migration scripts after the drop. |
| --env<br>--environment <environment> | _(empty)_ | Environment Name - This allows grate to be environment aware and only run scripts that are in a particular environment based on the name of the script.  'something.ENV.LOCAL.sql' would only be run if --env=LOCAL was set. |
| -w<br>--warnononetimescriptchanges | false | **WarnOnOneTimeScriptChanges** - Instructs grate to execute changed one time scripts(DDL / DML in Upfolder) that have previously been run against the database instead of failing. A warning is logged for each one time script that is rerun. |
| --warnandignoreononetimescriptchanges | false | **WarnAndIgnoreOnOneTimeScriptChanges** - Instructs grate to ignore and update the hash of changed one time scripts (DDL/DML in Up folder) that have previously been run against the database instead of failing. A warning is logged for each one time scripts that is rerun. |
| --disabletokenreplacement<br>--disabletokens | false | **Tokens** - This instructs grate to not perform token replacement ({{somename}}). |
| --usertokens<br>--ut &lt;usertokens&gt; | - | **User Tokens** - Allows grate to perform token replacement on custom tokens ({{my_token}}). Set as a key=value pair, eg '--ut=my_token=myvalue'. Can be specified multiple times. |
| --donotstorescriptsruntext | false | **DoNotStoreScriptsRunText** - This instructs grate to not store the full script text in the database. |
| --forceanytimescripts<br>--runallanytimescripts | false | **RunAllAnyTimeScripts** - This instructs grate to run any time scripts every time it is run even if they haven't changed. Defaults to false.
| --baseline | - | **Baseline** - This instructs grate to mark the scripts as run, but not to actually run anything against the database. Use this option if you already have scripts that have been run through other means (and BEFORE you start the new ones). | 
| --dryrun | false | **DryRun** - This instructs grate to log what would have run, but not to actually run anything against the database.  Use this option if you are trying to figure out what grate is going to do. |
| --restore | - | **Restore** - This instructs grate where to find the database backup file (.bak) to restore from. If this option is not specified, no restore will be done.
| -ni<br>--noninteractive<br>--silent | false | **Silent** - tells grate not to ask for any input when it runs.
| --version <version> | 1.0.0.0 | **Database Version** - specify the version of the current migration directly on the command line. |
| -v<br>--verbosity &lt;Critical\|<br>Debug\|<br>Error\|<br>Information\|<br>None\|<br>Trace\|Warning&gt; | Information | **Verbosity level** (as defined here: https://docs.microsoft.com/dotnet/api/Microsoft.Extensions.Logging.LogLevel)
| -?<br>-h<br>--help | - |  Show help and usage information | 
