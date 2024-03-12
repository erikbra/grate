using Oracle.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleGrateTestContext))]
public class Failing_Scripts(OracleGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Failing_Scripts(testContext, testOutput)
{
    protected override string ExpectedErrorMessageForInvalidSql =>
        """
        Update ("up"):
        --------------------------------------------------------------------------------
        2_failing.sql: ORA-00923: FROM keyword not found where expected
        (0,10): SELECT TOP
                         ^ ORA-00923: FROM keyword not found where expected
        """;

    protected override IDictionary<string, object?> ExpectedErrorDetails => new Dictionary<string, object?>
    {
        {"Message", "ORA-00923: FROM keyword not found where expected"},
        {"ParseErrorOffset", 10},
        {"Number", 923},
        {"Procedure", ""},
        {"DataSource", ""},
        {"Source", "Oracle Data Provider for .NET, Managed Driver"},
        {"ErrorCode", -2147467259},
        {"IsTransient", false},
        {"HelpLink", null},
        {"HResult", -2147467259},
    };
}
