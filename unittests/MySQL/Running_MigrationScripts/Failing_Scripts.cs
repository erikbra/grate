using MySQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace MySQL.Running_MigrationScripts;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts(MySqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Failing_Scripts(testContext, testOutput)
{
    protected override string ExpectedStartOfErrorMessageForInvalidSql => 
        """
        Update ("up"):
        --------------------------------------------------------------------------------
        2_failing.sql: Unknown column 'TOP' in 'field list'
        """;
    

    protected override IDictionary<string, object?> ExpectedErrorDetails => new Dictionary<string, object?>
    {
        {"Server Error Code", 1054},
        {"SqlState", "42S22"}
    };
}
