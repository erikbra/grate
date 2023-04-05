namespace grate.Configuration;

public record KnownFolderNames: IKnownFolderNames
{
    public string BeforeMigration { get; init; } = "beforeMigration";
    public string CreateDatabase { get; init; } = "createDatabase";
    public string DropDatabase { get; init; } = "dropDatabase";
    public string AlterDatabase { get; init; } = "alterDatabase";
    public string RunAfterCreateDatabase { get; init; } = "runAfterCreateDatabase";
    public string RunBeforeUp { get; init; } = "runBeforeUp";
    public string Up { get; init; } = "up";
    public string RunFirstAfterUp { get; init; } = "runFirstAfterUp";
    public string Functions { get; init; } = "functions";
    public string Views { get; init; } = "views";
    public string Sprocs { get; init; } = "sprocs";
    public string Triggers { get; init; } = "triggers";
    public string Indexes { get; init; } = "indexes";
    public string RunAfterOtherAnyTimeScripts { get; init; } = "runAfterOtherAnyTimeScripts";
    public string Permissions { get; init; } = "permissions";
    public string AfterMigration { get; init; } = "afterMigration";

    public static KnownFolderNames Default => new();
}
