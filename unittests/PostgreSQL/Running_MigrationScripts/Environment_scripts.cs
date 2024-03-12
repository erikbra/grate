using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[Collection(nameof(PostgreSqlGrateTestContext))]

// ReSharper disable once InconsistentNaming
public class Environment_scripts(PostgreSqlGrateTestContext testContext, ITestOutputHelper testOutput)
    : TestCommon.Generic.Running_MigrationScripts.Environment_scripts(testContext, testOutput);
