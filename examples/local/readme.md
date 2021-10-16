# Local example

This example demonstrates how to run grate against a local sql instance (targeting [localdb](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15) by default to simplify connection strings).

We haven't included every possible folder that grate can process here.  This is completely normal, and most real projects just use the folders they require.  The grate output includes the directories it's looking for (in order) if you need to run other types of scripts.

The files in each directory are processed in alphabetical order, and it's generally worth using some sort of always incrementing naming standard for the `up` scripts to ensure things are run in the correct order.

For more info on what each of the folders can be used for, and the different ways in which grate handles the files in each folder please [have a look at the documentation](https://erikbra.github.io/grate/getting-started/).

## Running the example

There's a `run-migration.ps1` file that:
 - Installs grate locally on the machine using [winget](https://docs.microsoft.com/en-us/windows/package-manager/winget/)
   - Winget was chosen for this example as we're targeting a sql localdb instance which kind of implies a windows developer machine.  If you're not on windows, or don't use Sql Server locally you might find the docker example more to your tastes :)
 - Runs a database migration against the sql server.

 
 ## Things to note

- The script tells grate that this is targeting the `local` environment, and so you'll notice the `002-Add-TestData.sql` script is not run.  If you change the script to use `--env=Test` instead you'll see the test data populated in the table.
- Feel free to experiment with adding more scripts/changing the views/sprocs and running with a new version number
- Note the tables added to the database (you can connect with SSMS/your db tool of choice) that audit exactly which scripts have been run, and when.