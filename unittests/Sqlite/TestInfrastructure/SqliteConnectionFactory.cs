using System.Data;
using Microsoft.Data.Sqlite;
using TestCommon.TestInfrastructure;

namespace Sqlite.TestInfrastructure;
public class SqliteConnectionFactory : IDatabaseConnectionFactory
{
    public IDbConnection GetDbConnection(string connectionString) => new SqliteConnection(connectionString);
}
