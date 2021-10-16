---
title: "Getting Started"
permalink: /getting-started/
---

# Getting Started

## Overview

#TODO




## Script Types

Scripts in grate are considered to be one of three types:

### One-time scripts
These are scripts that are run **exactly once** per database, and never again.  More info [here](/script-types/one-time).

### Anytime Scripts
These scripts are run **any time they're changed**.  More info [here](/script-types/anytime).

### Everytime Scripts
These scripts are run (you guessed it) **every time** :)  More info [here](/script-types/everytime)

## Directory run order

grate processes the files in a standard set of directories in a fixed order for deterministic processing.  Folders are run in the following order:

### 1. BeforeMigration (Everytime scripts)
If you have particular tasks you want to perform prior to any database migrations (custom logging? database backups? disable replication?) you can do it here.

### 2. AlterDatabase (Anytime scripts)
If you have scripts that need to alter the database config itself (rather than the _contents_ of the database) thjis is the place to do it.  For example setting recovery modes, enabling query stores, etc etc

### 3. RunAfterCreateDatabase (Anytime scripts).
This directory is only processed if the database was created from scratch by grate.  Maybe you need to add user accounts or similar?

### 4. RunBeforeUp (Anytime scripts)

### 5. Up (One-time scripts).  
This is where the bulk of your 'migration' scripts end up, eg adding tables, removing columns, adding reference data etc

### 6. RunFirstAfterUp (One-time scripts)
Scripts run prior to other anytime folders are found here. This folder exists to allow you to put sql files in when you need to run out of order, say a stored procecure prior to a function. It is not a normal occurrence to have many files in here or any for that matter.

### 7. Functions (Anytime scripts)
If you have any functions that need to run prior to others, make sure they are alphabetically first before the dependent scripts.

### 8. Views (Anytime scripts)
If you have views any that need to run prior to others, make sure they are alphabetically first before the dependent scripts.

### 9. Sprocs (Anytime scripts)
Stored procedures are found in a `sprocs` folder. If you have any that need to run prior to others, make sure they are alphabetically first before the dependent scripts.

### 10. Triggers (Anytime scripts)

### 11. Indexes (Anytime scripts)

### 12. RunAfterOtherAnyTimeScripts (Anytime scripts)
This folder exists to allow you to run scripts after you have set up your anytime scripts. It's pretty open what you put in here, but remember that it is still an anytime folder.

### 13. Permissions (Everytime scripts)
If you have any that need to run prior to others, make sure they are alphabetically first before the dependent scripts.
Permissions may contain auto-wiring of permissions, so they are run every time regardless of changes in the files.

### 14. AfterMigration (Everytime scripts)
If you have particular tasks you want to perform prior to any database migrations (custom logging? database backups?) you can do it here.

### Notes
- You are not required to have every one of these folders...i.e. if you don't use triggers there's no need to have an empty     `triggers` directory.