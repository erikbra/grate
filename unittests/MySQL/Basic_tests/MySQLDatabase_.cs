using FluentAssertions;
using grate.Configuration;
using MySQL.TestInfrastructure;
using MySqlConnector;

namespace MySQL.Basic_tests;

// ReSharper disable once InconsistentNaming
public class MySqlDatabase_(InspectableMySqlDatabase mySql)
{
    [Fact]
    public async Task Disables_pipelining_if_not_explicitly_set_in_connection_string()
    {
        var connStr = "Server=dummy";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        await mySql.InitializeConnections(cfg);

        var conn = mySql.GetConnection();
        var builder = new MySqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pipelining.Should().BeFalse();
    }

    [Fact]
    public async Task Leaves_pipelining_as_configured_if_set_explicitly_in_connection_string()
    {
        var connStr = "Server=dummy;Pipelining=true";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        await mySql.InitializeConnections(cfg);

        var conn = mySql.GetConnection();
        var builder = new MySqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pipelining.Should().BeTrue();
    }

}
