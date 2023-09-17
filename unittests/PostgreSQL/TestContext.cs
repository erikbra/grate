using NUnit.Framework;
using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

// There are some parallelism issues, but this does not solve it
//[assembly:LevelOfParallelism(1)]

namespace TestCommon;

public static class GrateTestContext
{
    internal static readonly PostgreSqlGrateTestContext PostgreSql = new();
}
