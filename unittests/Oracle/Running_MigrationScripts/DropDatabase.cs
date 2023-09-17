using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

public class DropDatabase : TestCommon.Generic.Running_MigrationScripts.DropDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}
