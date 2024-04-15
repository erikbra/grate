using PostgreSQL.TestInfrastructure;

namespace PostgreSQL.Reported_issues.Non_ascii_characters_in_script;

[Collection(nameof(PostgreSqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class ScriptsRunErrorsTable(PostgreSqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Reported_issues.Non_ascii_characters_in_script.ScriptsRunErrorsTable(testContext, testOutput);

