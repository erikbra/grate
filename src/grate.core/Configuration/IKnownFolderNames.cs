namespace grate.Configuration;

public interface IKnownFolderNames
{
    string BeforeMigration { get; }
    string CreateDatabase { get; }
    string DropDatabase { get; }
    string AlterDatabase { get; }
    string RunAfterCreateDatabase { get; }
    string RunBeforeUp { get; }
    string Up { get; }
    string RunFirstAfterUp { get; }
    string Functions { get; }
    string Views { get; }
    string Sprocs { get; }
    string Triggers { get; }
    string Indexes { get; }
    string RunAfterOtherAnyTimeScripts { get; }
    string Permissions { get; }
    string AfterMigration { get; }
}
