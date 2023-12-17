using System.Runtime.CompilerServices;
using grate.Configuration;

namespace TestCommon.TestInfrastructure;

public static class DescriptiveTestObjects
{
    public static MigrationsFolderWithDescription? Describe(MigrationsFolder? folder,
        [CallerArgumentExpression(nameof(folder))] string description = "") =>
        folder is { } ? new MigrationsFolderWithDescription(folder, description) : null;
}

public record MigrationsFolderWithDescription: MigrationsFolder
{
    public MigrationsFolderWithDescription(MigrationsFolder baseFolder, string description) : base(baseFolder)
    {
        Description = description;
    }

    private string Description { get; } = null!;
    public override string ToString() => Description;
}
