using grate.unittests.TestInfrastructure;

namespace grate.unittests.Oracle.Running_MigrationScripts;

public class DropDatabase : Generic.Running_MigrationScripts.DropDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}