using System.Data;
using Microsoft.Data.SqlClient;
using TestCommon.TestInfrastructure;

namespace SqlServer.TestInfrastructure;
public class SqlServerConnectionFactory : IDatabaseConnectionFactory
{
    public IDbConnection GetDbConnection(string connectionString) => new SqlConnection(connectionString);
}
