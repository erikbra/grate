using grate.Infrastructure;
using grate.Oracle.Migration;
using Oracle.TestInfrastructure;
using TestCommon.DependencyInjection;
using TestCommon.TestInfrastructure;

namespace Oracle.DependencyInjection;

[Collection(nameof(OracleGrateTestContext))]
public class ServiceCollectionTest(OracleGrateTestContext testContext)
    : GrateServiceCollectionTest(testContext)
{
    protected virtual Type DatabaseType => typeof(OracleDatabase);
    protected virtual ISyntax Syntax => OracleDatabase.Syntax;
}
