using MySQL.TestInfrastructure;

namespace MySQL.Reported_issues.Non_ascii_characters_in_script;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class ScriptsRunTable(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Reported_issues.Non_ascii_characters_in_script.ScriptsRunTable(testContext, testOutput);

