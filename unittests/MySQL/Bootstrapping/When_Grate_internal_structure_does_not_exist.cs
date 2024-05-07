using MySQL.TestInfrastructure;

namespace MySQL.Bootstrapping;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class When_Grate_internal_structure_does_not_exist(MySqlGrateTestContext context, ITestOutputHelper testOutput) 
    : TestCommon.Generic.Bootstrapping.When_Grate_internal_structure_does_not_exist(context, testOutput);

