using PostgreSQL.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.DependencyInjection;

[Collection(nameof(PostgreSqlGrateTestContext))]
public class ServiceCollectionTest(PostgreSqlGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context)
{
    protected override string VarcharType => "varchar";
    protected override string BigintType => "BIGINT";
}
