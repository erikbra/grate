using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

public class ScriptsRun_Table: TestCommon.Generic.Running_MigrationScripts.ScriptsRun_Table
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
