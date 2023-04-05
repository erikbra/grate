using System.Data.Common;
using System.Threading.Tasks;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace grate.unittests.Basic.Infrastructure.SqlServer;

// ReSharper disable once InconsistentNaming
public class SqlServerDatabase_
{
    [Test]
    public async Task Disables_pooling_if_not_explicitly_set_in_connection_string()
    {
        var connStr = "Server=dummy";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        var sqlServerDatabase = new InspectableSqlServerDatabase();
        await sqlServerDatabase.InitializeConnections(cfg);

        var conn = sqlServerDatabase.GetConnection();
        var builder = new SqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pooling.Should().BeFalse();
    }
    
    [Test]
    public async Task Leaves_pooling_as_configured_if_set_explicitly_in_connection_string()
    {
        var connStr = "Server=dummy;Pooling=true";
        var cfg = new GrateConfiguration() { ConnectionString = connStr };
        var sqlServerDatabase = new InspectableSqlServerDatabase();
        await sqlServerDatabase.InitializeConnections(cfg);

        var conn = sqlServerDatabase.GetConnection();
        var builder = new SqlConnectionStringBuilder(conn.ConnectionString);
        builder.Pooling.Should().BeTrue();
    }

    private class InspectableSqlServerDatabase : SqlServerDatabase
    {
        public InspectableSqlServerDatabase() : base(TestConfig.LogFactory.CreateLogger<SqlServerDatabase>())
        {
        }

        public DbConnection GetConnection() => base.Connection;
    }
}
