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

1. BeforeMigration (Everytime scripts)
2. AlterDatabase (Anytime scripts)
3. RunAfterCreateDatabase (Anytime scripts).  This directory is only processed if the database was created from scratch.
4. RunBeforeUp (Anytime scripts)
5. Up (One-time scripts).  This is where the bulk of your 'migration' scripts end up, eg adding tables, removing columns, adding reference data etc
6. RunFirstAfterUp (One-time scripts)
7. Functions (Anytime scripts)
8. Views (Anytime scripts)
9. Sprocs (Anytime scripts)
10. Triggers (Anytime scripts)
11. Indexes (Anytime scripts)
12. RunAfterOtherAnyTimeScripts (Anytime scripts)
13. Permissions (Everytime scripts)
14. AfterMigration (Everytime scripts)