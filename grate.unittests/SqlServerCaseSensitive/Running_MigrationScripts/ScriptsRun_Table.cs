﻿using grate.unittests.TestInfrastructure;

namespace grate.unittests.SqlServerCaseSensitive.Running_MigrationScripts
{
    public class ScriptsRun_Table : Generic.Running_MigrationScripts.ScriptsRun_Table
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
