using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL.Running_MigrationScripts;

public class DropDatabase : Generic.Running_MigrationScripts.DropDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
