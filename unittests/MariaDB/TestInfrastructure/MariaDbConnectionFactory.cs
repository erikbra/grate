using System.Data;
using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MariaDB.TestInfrastructure;
public class MariaDbConnectionFactory : IDatabaseConnectionFactory
{
    public IDbConnection GetDbConnection(string connectionString) => new MySqlConnection(connectionString);
}
