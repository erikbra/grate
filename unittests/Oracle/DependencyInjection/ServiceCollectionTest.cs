using grate.Infrastructure;
using grate.Oracle.Migration;
using TestCommon.DependencyInjection;
using TestCommon.TestInfrastructure;

namespace Oracle.DependencyInjection;

[Collection(nameof(OracleTestContainer))]
public class ServiceCollectionTest(IGrateTestContext testContext)
    : GrateServiceCollectionTest(testContext)
{
    protected virtual Type DatabaseType => typeof(OracleDatabase);
    protected virtual ISyntax Syntax => OracleDatabase.Syntax;
}
