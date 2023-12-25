using System.Data;

namespace TestCommon.TestInfrastructure;
public interface IDatabaseConnectionFactory
{
    IDbConnection GetDbConnection(string connectionString);
}
