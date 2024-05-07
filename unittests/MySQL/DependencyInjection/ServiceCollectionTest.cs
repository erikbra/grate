using MySQL.TestInfrastructure;
using TestCommon.TestInfrastructure;
namespace MySQL.DependencyInjection;

[Collection(nameof(MySqlGrateTestContext))]
// ReSharper disable once UnusedType.Global
public class ServiceCollectionTest(MySqlGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context)
{
    protected override string VarcharType => "varchar";
    protected override string BigintType => "BIGINT";
}
