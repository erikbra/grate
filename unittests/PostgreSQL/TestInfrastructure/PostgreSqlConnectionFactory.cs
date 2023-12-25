using System.Data;
using Npgsql;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.TestInfrastructure;
public class PostgreSqlConnectionFactory : IDatabaseConnectionFactory
{
    public IDbConnection GetDbConnection(string connectionString) => new NpgsqlConnection(connectionString);
}
