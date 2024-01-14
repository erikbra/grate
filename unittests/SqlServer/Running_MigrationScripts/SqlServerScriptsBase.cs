using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

public abstract class SqlServerScriptsBase(IGrateTestContext context, ITestOutputHelper testOutput) :
    MigrationsScriptsBase(context, testOutput);

