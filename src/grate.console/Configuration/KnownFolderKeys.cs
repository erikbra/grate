using System.Collections.Generic;

namespace grate.Configuration;

public static class KnownFolderKeys
{
    public const string BeforeMigration = nameof(BeforeMigration);
    public const string CreateDatabase = nameof(CreateDatabase);
    public const string AlterDatabase = nameof(AlterDatabase);
    public const string RunAfterCreateDatabase = nameof(RunAfterCreateDatabase);
    public const string RunBeforeUp = nameof(RunBeforeUp);
    public const string Up = nameof(Up);
    public const string RunFirstAfterUp = nameof(RunFirstAfterUp);
    public const string Functions = nameof(Functions);
    public const string Views = nameof(Views);
    public const string Sprocs = nameof(Sprocs);
    public const string Triggers = nameof(Triggers);
    public const string Indexes = nameof(Indexes);
    public const string RunAfterOtherAnyTimeScripts = nameof(RunAfterOtherAnyTimeScripts);
    public const string Permissions = nameof(Permissions);
    public const string AfterMigration = nameof(AfterMigration);

    public static readonly IEnumerable<string> Keys = new[]
    {
        BeforeMigration, AlterDatabase, RunAfterCreateDatabase, RunBeforeUp, Up, RunFirstAfterUp, Functions, Views,
        Sprocs, Triggers, Indexes, RunAfterOtherAnyTimeScripts, Permissions, AfterMigration
    };
}
