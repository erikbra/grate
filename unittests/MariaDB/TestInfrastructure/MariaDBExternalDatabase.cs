using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MariaDB.TestInfrastructure;

public class MariaDBExternalDatabase(GrateTestConfig grateTestConfig) : ITestDatabase
{
    public string AdminConnectionString { get; } = grateTestConfig.AdminConnectionString ?? throw new ArgumentNullException(nameof(grateTestConfig));

    public string ConnectionString(string database)
    {
        var builder = new MySqlConnectionStringBuilder(AdminConnectionString)
        {
            Database = database,
            ConnectionTimeout = 2
        };
        return builder.ConnectionString;
    }

    public string UserConnectionString(string database)
    {
        var builder = new MySqlConnectionStringBuilder(AdminConnectionString)
        {
            Database = database,
            UserID = "zoobat",
            Password = "batmanZZ5",
            ConnectionTimeout = 2
        };
         return builder.ConnectionString;
    }
    
}
