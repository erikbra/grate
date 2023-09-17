using Sqlite.TestInfrastructure;

// There are some parallelism issues, but this does not solve it
//[assembly:LevelOfParallelism(1)]

namespace Sqlite;

public static class GrateTestContext
{
    internal static readonly SqliteGrateTestContext Sqlite = new();
}
