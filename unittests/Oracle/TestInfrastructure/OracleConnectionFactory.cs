using System.Data;
using Oracle.ManagedDataAccess.Client;
using TestCommon.TestInfrastructure;

namespace Oracle.TestInfrastructure;
public class OracleConnectionFactory : IDatabaseConnectionFactory
{
    public IDbConnection GetDbConnection(string connectionString) => new OracleConnection(connectionString);
}
