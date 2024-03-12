using SqlServer.TestInfrastructure;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

public abstract class SqlServerScriptsBase(SqlServerGrateTestContext context, ITestOutputHelper testOutput) :
    MigrationsScriptsBase(context, testOutput);

