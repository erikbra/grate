using TestCommon.TestInfrastructure;

namespace TestCommon.PostgreSQL.Running_MigrationScripts;

public class ScriptsRun_Table: Generic.Running_MigrationScripts.ScriptsRun_Table
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
