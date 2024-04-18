using Sqlite.TestInfrastructure;

namespace Sqlite.Reported_issues.Reported_issues.Non_ascii_characters_in_script;

[Collection(nameof(SqliteGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class ScriptsRunErrorsTable(SqliteGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Reported_issues.Non_ascii_characters_in_script.ScriptsRunErrorsTable(testContext, testOutput);

