---
title: "One-Time Scripts"
permalink: script-types/one-time/
parent: Script types
---
## One Time Scripts

These are scripts that are run **exactly once** per database, and never again.  Think things like `create table...` statements.  You can't create a table twice so any attempt to run the script a second time results in an error.

## Folders
Scripts in the following folders are `one time` scripts by default.

* RunAfterCreateDatabase
* Up

## What goes in the Up Folder?

1. DDL (schema changes - database structure)  
1. DML (inserts/updates/deletes)  
  
## How does the order work?
  
grate always runs files in order alphabetically.
  
## How should I name my scripts?
  
One should prepend your order specific scripts with either a number moving upwards padded with three zeros (i.e. `0001_somescript.sql` followed by `0002_nextscript.sql`) or a nice long date time in YYYYMMddHHmmss format (i.e. `20121026091400_somescript.sql` followed by `20121026091401_nextscript.sql`), but you are not limited to those options. Some people do a separation by numbers as in `####.##.##.####` or something else. Find what works for you (and your team) and use it.   
  
## Errors
If there is a change to a one time script and the migrator is run, grate will determine you have changed that file and will shut down immediately with errors.  That being said, there is a configuration setting to allow you to still run with warnings. Although not recommended, grate tries not to be a tool that constrains users.  
See [[ConfigurationOptions]] under **Switches** `WarnOnOneTimeScriptChanges`
