using System;
using System.IO;

namespace grate.Configuration;

// ReSharper disable once InconsistentNaming
public static class DefaultConfiguration
{
    public static readonly string DefaultAlterDatabaseFolderName = "alterDatabase";
    public static readonly string DefaultRunAfterCreateDatabaseFolderName = "runAfterCreateDatabase";
    public static readonly string DefaultRunBeforeUpFolderName = "runBeforeUp";
    public static readonly string DefaultUpFolderName = "up";
    public static readonly string DefaultDownFolderName = "down";
    public static readonly string DefaultRunFirstAfterUpFolderName = "runFirstAfterUp";
    public static readonly string DefaultFunctionsFolderName = "functions";
    public static readonly string DefaultViewsFolderName = "views";
    public static readonly string DefaultSprocsFolderName = "sprocs";
    public static readonly string DefaultTriggersFolderName = "triggers";
    public static readonly string DefaultIndexesFolderName = "indexes";
    public static readonly string DefaultRunAfterOtherAnyTimeFolderName = "runAfterOtherAnyTimeScripts";
    public static readonly string DefaultPermissionsFolderName = "permissions";
    public static readonly string DefaultBeforeMigrationFolderName = "beforeMigration";
    public static readonly string DefaultAfterMigrationFolderName = "afterMigration";
    public static readonly string DefaultEnvironmentName = "LOCAL";
    public static readonly string DefaultGrateSchemaName = "grate";
    public static readonly string DefaultVersionTableName = "Version";
    public static readonly string DefaultScriptsRunTableName = "ScriptsRun";
    public static readonly string DefaultScriptsRunErrorsTableName = "ScriptsRunErrors";
    public static readonly string DefaultVersionFile = @"_BuildInfo.xml";
    public static readonly string DefaultVersionXPath = @"//buildInfo/version";
    public static readonly string DefaultFilesDirectory = @".";
    public static readonly string DefaultServerName = "(local)";
    public static readonly string DefaultOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create), "grate");
    public static readonly string LoggingFile = @"C:\Temp\grate\grate.changes.log";
    public static readonly int DefaultCommandTimeout = 60;
    public static readonly int DefaultAdminCommandTimeout = 300;
    public static readonly int DefaultRestoreTimeout = 900;

    public static readonly bool DefaultDisableOutput = false;

}
