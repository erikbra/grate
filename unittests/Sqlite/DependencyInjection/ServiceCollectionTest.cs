using grate.Infrastructure;
using grate.Sqlite.Migration;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.DependencyInjection;
public class ServiceCollectionTest(SqliteGrateTestContext context) : TestCommon.DependencyInjection.GrateServiceCollectionTest(context)
{
    protected virtual Type DatabaseType => typeof(SqliteDatabase);
    protected virtual ISyntax Syntax => SqliteDatabase.Syntax;
}
