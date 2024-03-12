﻿using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.Running_MigrationScripts;

[Collection(nameof(SqlServerGrateTestContext))]
// ReSharper disable once InconsistentNaming
public class ScriptsRun_Table(IGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.ScriptsRun_Table(testContext, testOutput);
