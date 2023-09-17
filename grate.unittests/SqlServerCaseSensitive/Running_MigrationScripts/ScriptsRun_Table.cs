using Unit_tests.TestInfrastructure;

namespace Unit_tests.SqlServerCaseSensitive.Running_MigrationScripts
{
    public class ScriptsRun_Table : Generic.Running_MigrationScripts.ScriptsRun_Table
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
