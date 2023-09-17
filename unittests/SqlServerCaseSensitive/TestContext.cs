using NUnit.Framework;
using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.TestInfrastructure;

// There are some parallelism issues, but this does not solve it
//[assembly:LevelOfParallelism(1)]

namespace SqlServerCaseSensitive;

public static class GrateTestContext
{
    internal static readonly SqlServerGrateTestContext SqlServer = new();
    internal static readonly SqlServerGrateTestContext SqlServerCaseSensitive = new("Latin1_General_CS_AS"); //CS == Case Sensitive
}
