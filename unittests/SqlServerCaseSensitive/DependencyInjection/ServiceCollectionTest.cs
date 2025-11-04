using SqlServerCaseSensitive.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.DependencyInjection;

[Collection(nameof(SqlServerGrateTestContext))]
public class ServiceCollectionTest(SqlServerGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context)
{
    protected override string VarcharType => "nvarchar";
    protected override string BigintType => "BIGINT";
}
