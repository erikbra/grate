using MySQL.TestInfrastructure;

namespace MySQL.Bootstrapping;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class When_Grate_structure_is_not_latest_version(MySqlGrateTestContext context, ITestOutputHelper testOutput) 
    : TestCommon.Generic.Bootstrapping.When_Grate_structure_is_not_latest_version(context, testOutput);

