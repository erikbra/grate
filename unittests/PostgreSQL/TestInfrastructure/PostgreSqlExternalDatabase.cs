using Npgsql;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.TestInfrastructure;

// ReSharper disable once ClassNeverInstantiated.Global
public class PostgreSqlExternalDatabase(GrateTestConfig grateTestConfig) : ITestDatabase
{
    public string AdminConnectionString { get; } = grateTestConfig.AdminConnectionString ?? throw new ArgumentNullException(nameof(grateTestConfig));

    public string ConnectionString(string database)
    {
        var builder = new NpgsqlConnectionStringBuilder(AdminConnectionString)
        {
            Database = database,
            Timeout = 2
        };
        return builder.ConnectionString;
    }

    public string UserConnectionString(string database)
    {
         var builder = new NpgsqlConnectionStringBuilder(AdminConnectionString)
         {
            Database = database,
            Username = "zorro",
            Password = "batmanZZ4",
            Timeout = 2
         };
         return builder.ConnectionString;
    }
    
}
