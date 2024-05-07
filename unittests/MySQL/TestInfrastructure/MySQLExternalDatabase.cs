using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MySQL.TestInfrastructure;

public class MySQLExternalDatabase(GrateTestConfig grateTestConfig) : ITestDatabase
{
    public string AdminConnectionString { get; } = grateTestConfig.AdminConnectionString ?? throw new ArgumentNullException(nameof(grateTestConfig));

    public string ConnectionString(string database)
    {
        var builder = new MySqlConnectionStringBuilder(AdminConnectionString)
        {
            Database = database,
            ConnectionTimeout = 5
        };
        return builder.ConnectionString;
    }

    public string UserConnectionString(string database)
    {
        var builder = new MySqlConnectionStringBuilder(AdminConnectionString)
        {
            Database = database,
            UserID = "zoobat" + database[..4],
            Password = "batmanZZ5",
            ConnectionTimeout = 5
        };
         return builder.ConnectionString;
    }
    
}
