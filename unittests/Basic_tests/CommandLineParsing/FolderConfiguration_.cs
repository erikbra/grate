using System.Collections.Immutable;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.Runtime.CompilerServices;
using FluentAssertions;
using grate.Commands;
using grate.Configuration;

namespace Basic_tests.CommandLineParsing;

// ReSharper disable once InconsistentNaming
public class FolderConfiguration_
{
    [Fact]
    public async Task Default()
    {
        var cfg = await ParseGrateConfiguration("--sqlfilesdirectory=/tmp");
        _ = cfg?.SqlFilesDirectory ?? new DirectoryInfo("/tmp");

        var expected = FoldersConfiguration.Default(null);
        var actual = cfg?.Folders;

        AssertEquivalent(expected.Values, actual?.Values);
    }

    [Theory]
    [MemberData(nameof(FoldersCommandLines))]
    public async Task Default_With_Overrides(string commandLine, KnownFolderNames folderConfig)
    {
        var cfg = await ParseGrateConfiguration("--sqlfilesdirectory=/tmp", commandLine);
        _ = cfg?.SqlFilesDirectory ?? new DirectoryInfo("/tmp");
        var expected = FoldersConfiguration.Default(folderConfig);

        var actual = cfg?.Folders;

        AssertEquivalent(expected.Values, actual?.Values);
    }

    [Theory]
    [MemberData(nameof(FullyCustomFoldersCommandLines))]
    public async Task Fully_Customised(string name, string root, string foldersArgument, IFoldersConfiguration expected)
    {
        var s = name;
        var cfg = await ParseGrateConfiguration("--sqlfilesdirectory=" + root, foldersArgument);
        var actual = cfg?.Folders;

        AssertEquivalent(expected.Values, actual?.Values);
    }

    public static TheoryData<string, KnownFolderNames> FoldersCommandLines() =>
        new()
        {
            { "--folders=up=tables", Wrap(KnownFolderNames.Default with { Up = "tables" })! },
            { "--folders=up=tables;views=projections", Wrap(KnownFolderNames.Default with { Up = "tables", Views = "projections" })! },
        };


    public static TheoryData<string, string, string, IFoldersConfiguration> FullyCustomFoldersCommandLines() =>
        new()
        {
            {
                "Mostly defaults",
                "/tmp/jalla",
                @"--folders=folder1=type:Once;folder2=type:EveryTime;folder3=type:AnyTime",
                new FoldersConfiguration(
                    new MigrationsFolder("folder1", MigrationType.Once),
                    new MigrationsFolder("folder2", MigrationType.EveryTime),
                    new MigrationsFolder("folder3", MigrationType.AnyTime)
                )
            }
        };


    private static void AssertEquivalent(
        IEnumerable<MigrationsFolder?> expected,
        IEnumerable<MigrationsFolder?>? actual)
    {
        var expectedArr = expected.ToImmutableArray();
        var actualArr = (actual ?? Enumerable.Empty<MigrationsFolder>()).ToImmutableArray();

        Assert.Multiple(() =>
        {
            actualArr.Length.Should().Be(expectedArr.Length);
            for (var i = 0; i < expectedArr.Length; i++)
            {
                AssertEquivalent(expectedArr[i], actualArr[i]);
            }
        });
    }

    private static void AssertEquivalent(MigrationsFolder? expected, MigrationsFolder? actual)
    {
        Assert.Multiple(() =>
        {
            actual?.Name.Should().Be(expected?.Name);
            actual?.Path?.Should().Be(expected?.Path);
            actual?.Type.Should().Be(expected?.Type);
            actual?.ConnectionType.Should().Be(expected?.ConnectionType);
            actual?.TransactionHandling.Should().Be(expected?.TransactionHandling);
        });
    }


    private static async Task<GrateConfiguration?> ParseGrateConfiguration(params string[] commandline)
    {
        GrateConfiguration? cfg = null;
        var cmd = CommandHandler.Create((GrateConfiguration config) => cfg = config);

        var migrateCommand = new MigrateCommand(null!);
        ParseResult p = new Parser(migrateCommand).Parse(commandline);
        await cmd.InvokeAsync(new InvocationContext(p));

        return cfg;
    }

    private static KnownFolderNamesWithDescription? Wrap(KnownFolderNames? names, [CallerArgumentExpression(nameof(names))] string description = "") =>
        names is { } ? new KnownFolderNamesWithDescription(names) { Description =  description} : null;

    public record KnownFolderNamesWithDescription : KnownFolderNames
    {
        public KnownFolderNamesWithDescription(KnownFolderNames folder) : base(folder)
        {
        }

        public string Description { get; init; } = null!;
        public override string ToString() => Description;
    }

}
