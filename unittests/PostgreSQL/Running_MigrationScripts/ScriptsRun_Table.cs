﻿using TestCommon;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

public class ScriptsRun_Table: TestCommon.Generic.Running_MigrationScripts.ScriptsRun_Table
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
