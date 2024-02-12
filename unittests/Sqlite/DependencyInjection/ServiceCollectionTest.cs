using grate.Infrastructure;
using grate.Sqlite.Migration;
using TestCommon.TestInfrastructure;

namespace Sqlite.DependencyInjection;
public class ServiceCollectionTest(IGrateTestContext context) : TestCommon.DependencyInjection.GrateServiceCollectionTest(context)
{
    protected virtual Type DatabaseType => typeof(SqliteDatabase);
    protected virtual ISyntax Syntax => SqliteDatabase.Syntax;
}
