---
title: "Anytime Scripts"
permalink: script-types/anytime/
parent: Script types
---
# Anytime Scripts

Anytime scripts are scripts that are run anytime they have changes. That means grate automatically detects new files and changes in files and runs when it finds changes.

## Folders
Scripts in the following folders are treated as `Anytime` scripts by default:

* AlterDatabase
* RunBeforeUp
* RunFirstAfterUpdate
* Functions
* Views
* Sprocs
* Indexes
* RunAfterOtherAnyTimeScripts

## Notes

Remember that as your script may be run multiple times, you can't just make it a `create...` script, but should use `create or alter...` instead.  If your DBMS version doesn't support `create or alter...` semantics then you can use a 'create if not exists, alter otherwise' workflow instead.

eg:
``` sql
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[ss].[usp_GetAllThis]'))
EXECUTE('CREATE PROC [ss].[usp_GetAllThis] AS SELECT * FROM sys.objects') -- Create a dummy placeholder object if needed

GO
-- You can then alter the proc as needed here over repeat migrations without losing permissions etc.

ALTER PROCEDURE [ss].[usp_GetAllThis] 
/* your procedure guts here */
```