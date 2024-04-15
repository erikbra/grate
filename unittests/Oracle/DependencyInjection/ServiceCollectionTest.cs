using Oracle.TestInfrastructure;
using TestCommon.DependencyInjection;

namespace Oracle.DependencyInjection;

[Collection(nameof(OracleGrateTestContext))]
public class ServiceCollectionTest(OracleGrateTestContext testContext)
    : GrateServiceCollectionTest(testContext)
{
    protected override string VarcharType => "VARCHAR2";
    protected override string BigintType => "NUMBER(19)";
}
