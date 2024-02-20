using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[Collection(nameof(OracleTestContainer))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class Versioning_The_Database(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database(testContext, testOutput)
{
    [Fact(Skip = "Skip due to Oracle doesn't support dynamic database creation in runtime")]
    public override Task Does_not_create_versions_when_no_script_existing()
    {
        return Task.CompletedTask;
    }
}

