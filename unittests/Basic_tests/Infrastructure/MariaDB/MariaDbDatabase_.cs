using System.Data.Common;
using System.Threading.Tasks;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using NUnit.Framework;

namespace Basic_tests.Infrastructure.MariaDB;

// ReSharper disable once InconsistentNaming
public class MariaDbDatabase_
{
    [Test]
    public async Task Disables_pipelining_if_not_explicitly_set_in_connection_string()
    {
        var connStr = "Server=dummy";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        var mariaDb = new InspectableMariaDbDatabase();
        await mariaDb.InitializeConnections(cfg);

        var conn = mariaDb.GetConnection();
        var builder = new MySqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pipelining.Should().BeFalse();
    }
    
    [Test]
    public async Task Leaves_pipelining_as_configured_if_set_explicitly_in_connection_string()
    {
        var connStr = "Server=dummy;Pipelining=true";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        var mariaDb = new InspectableMariaDbDatabase();
        await mariaDb.InitializeConnections(cfg);

        var conn = mariaDb.GetConnection();
        var builder = new MySqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pipelining.Should().BeTrue();
    }

    private class InspectableMariaDbDatabase : MariaDbDatabase
    {
        public InspectableMariaDbDatabase() : base(TestConfig.LogFactory.CreateLogger<InspectableMariaDbDatabase>())
        {
        }

        public DbConnection GetConnection() => base.Connection;
    }
}
