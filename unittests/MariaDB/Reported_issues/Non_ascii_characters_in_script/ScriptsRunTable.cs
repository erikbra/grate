using MariaDB.TestInfrastructure;

namespace MariaDB.Reported_issues.Non_ascii_characters_in_script;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class ScriptsRunTable(MariaDbGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Reported_issues.Non_ascii_characters_in_script.ScriptsRunTable(testContext, testOutput);

