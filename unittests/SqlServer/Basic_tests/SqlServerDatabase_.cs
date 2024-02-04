using FluentAssertions;
using grate.Configuration;
using Microsoft.Data.SqlClient;
using SqlServer.TestInfrastructure;

namespace SqlServer.Basic_tests;

// ReSharper disable once InconsistentNaming
public class SqlServerDatabase_(InspectableSqlServerDatabase sqlServerDatabase)
{
    [Fact]
    public async Task Disables_pooling_if_not_explicitly_set_in_connection_string()
    {
        var connStr = "Server=dummy";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        await sqlServerDatabase.InitializeConnections(cfg);

        var conn = sqlServerDatabase.GetConnection();
        var builder = new SqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pooling.Should().BeFalse();
    }

    [Fact]
    public async Task Leaves_pooling_as_configured_if_set_explicitly_in_connection_string()
    {
        var connStr = "Server=dummy;Pooling=true";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        await sqlServerDatabase.InitializeConnections(cfg);

        var conn = sqlServerDatabase.GetConnection();
        var builder = new SqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pooling.Should().BeTrue();
    }
}
