using System.Data.Common;
using FluentAssertions;
using grate.Configuration;
using grate.MariaDb.Migration;
using MariaDB.TestInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Basic_tests.Infrastructure.MariaDB;

// ReSharper disable once InconsistentNaming
public class MariaDbDatabase_ : IClassFixture<DependencyService>
{
    private IServiceProvider _serviceProvider;

    public MariaDbDatabase_(DependencyService dependencyService)
    {
        _serviceProvider = dependencyService.ServiceProvider;
    }
    [Fact]
    public async Task Disables_pipelining_if_not_explicitly_set_in_connection_string()
    {
        var connStr = "Server=dummy";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        var mariaDb = new InspectableMariaDbDatabase(_serviceProvider);
        await mariaDb.InitializeConnections(cfg);

        var conn = mariaDb.GetConnection();
        var builder = new MySqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pipelining.Should().BeFalse();
    }

    [Fact]
    public async Task Leaves_pipelining_as_configured_if_set_explicitly_in_connection_string()
    {
        var connStr = "Server=dummy;Pipelining=true";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        var mariaDb = new InspectableMariaDbDatabase(_serviceProvider);
        await mariaDb.InitializeConnections(cfg);

        var conn = mariaDb.GetConnection();
        var builder = new MySqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pipelining.Should().BeTrue();
    }

    private class InspectableMariaDbDatabase : MariaDbDatabase
    {
        public InspectableMariaDbDatabase(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<ILogger<InspectableMariaDbDatabase>>())
        {
        }

        public DbConnection GetConnection() => base.Connection;
    }
}
