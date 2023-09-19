using TestCommon;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

public class DropDatabase : TestCommon.Generic.Running_MigrationScripts.DropDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
