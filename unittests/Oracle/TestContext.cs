using NUnit.Framework;
using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

// There are some parallelism issues, but this does not solve it
//[assembly:LevelOfParallelism(1)]

namespace Oracle;

public static class GrateTestContext
{
    internal static readonly OracleGrateTestContext Oracle = new();
}
