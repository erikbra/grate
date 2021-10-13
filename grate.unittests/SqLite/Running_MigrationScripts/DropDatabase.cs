using grate.unittests.TestInfrastructure;

namespace grate.unittests.Sqlite.Running_MigrationScripts
{
    public class DropDatabase : Generic.Running_MigrationScripts.DropDatabase
    {
        protected override IGrateTestContext Context => GrateTestContext.Sqlite;
    }
}
