using System.Data.Common;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
using Xunit;

namespace Basic_tests.Infrastructure.SqlServer;

// ReSharper disable once InconsistentNaming
public class SqlServerDatabase_ : IClassFixture<SimpleService>
{
    private IServiceProvider _serviceProvider;

    public SqlServerDatabase_(SimpleService simpleService)
    {
        _serviceProvider = simpleService.ServiceProvider;
    }
    [Fact]
    public async Task Disables_pooling_if_not_explicitly_set_in_connection_string()
    {
        var connStr = "Server=dummy";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        var sqlServerDatabase = new InspectableSqlServerDatabase(_serviceProvider);
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
        var sqlServerDatabase = new InspectableSqlServerDatabase(_serviceProvider);
        await sqlServerDatabase.InitializeConnections(cfg);

        var conn = sqlServerDatabase.GetConnection();
        var builder = new SqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pooling.Should().BeTrue();
    }

    private class InspectableSqlServerDatabase : SqlServerDatabase
    {
        public InspectableSqlServerDatabase(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<ILogger<SqlServerDatabase>>())
        {
        }

        public DbConnection GetConnection() => base.Connection;
    }
}
