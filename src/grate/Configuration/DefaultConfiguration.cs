namespace grate.Configuration;

// ReSharper disable once InconsistentNaming
internal static class DefaultConfiguration
{
    public const string DefaultEnvironmentName = "LOCAL";
    public const string DefaultGrateSchemaName = "grate";
    public const string DefaultVersionTableName = "Version";
    public const string DefaultScriptsRunTableName = "ScriptsRun";
    public const string DefaultScriptsRunErrorsTableName = "ScriptsRunErrors";
    public const string DefaultVersionFile = @"_BuildInfo.xml";
    public const string DefaultVersionXPath = @"//buildInfo/version";
    public const string DefaultFilesDirectory = @".";
    public const string DefaultServerName = "(local)";
    public static readonly string DefaultOutputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "grate");
    public static readonly string LoggingFile = Path.Combine(Path.GetTempPath(), ApplicationInfo.Name, $"{ApplicationInfo.Name}.changes.log");
    public const int DefaultCommandTimeout = 60;
    public const int DefaultAdminCommandTimeout = 300;
    public const int DefaultRestoreTimeout = 900;

}
