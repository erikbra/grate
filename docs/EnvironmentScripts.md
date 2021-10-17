---
title: "Environment Specific Scripts"
permalink: /environment-scripts/
---
# Environment Specific Scripts

If you name a sql file with `*.ENV.*`, grate will start looking for the environment name somewhere on the file. If it doesn't find the proper environment, it will not run the file.  

In example, if you name a file **LOCAL.permissionfile.env.sql** and it is migrating the `LOCAL` environment, it will run the file. But if it is migrating the `TEST` environment, it will pass over this file.  

You configure the environment grate is targeting via the `--env` or `--environment` command line option, eg `> grate --env=LOCAL`.

## Why would I use this?
This is a handy way to insert test data or set custom permissions or settings for different environments.

## Does this support multiple environments

Yes and No.

Files can target multiple environments just fine, eg `AddTestData.Env.TEST.UAT.sql` will be run into either the `Test` or `UAT` environments.

Roundhouse supported a single _migration_ targeting multiple environments, however grate currently does not allow this, i.e. at most one `--env` argument is allowed.  If this causes you pain please raise an issue to discuss!

## Does this lock me into grate?
Only a little, but not really (not like scripts with [tokens](/token-replacement) in them). If you find yourself in a situation where you needed to manually run scripts, only run the scripts for the particular environment.