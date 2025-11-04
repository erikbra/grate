using SqlServerCaseSensitive.TestInfrastructure;

namespace SqlServerCaseSensitive.Bootstrapping;

[Collection(nameof(SqlServerGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class When_Grate_structure_is_not_latest_version(SqlServerGrateTestContext context, ITestOutputHelper testOutput) 
    : TestCommon.Generic.Bootstrapping.When_Grate_structure_is_not_latest_version(context, testOutput);


