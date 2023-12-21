using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TestCommon.TestInfrastructure;

namespace Basic_tests;

// ReSharper disable once InconsistentNaming
public class GrateConfiguration_ : IClassFixture<SimpleService>
{
    private IServiceProvider _serviceProvider;
    public GrateConfiguration_(SimpleService simpleService)
    {
        _serviceProvider = simpleService.ServiceProvider;
    }
    [Theory]
    [InlineData(grate.SqlServer.Infrastructure.DatabaseType.Name)]
    public void Uses_ConnectionString_with_master_db_if_adminConnectionString_is_not_set_Initial_Catalog(string databaseType)
    {
        var cfg = new GrateConfiguration()
        { ConnectionString = "Data source=Monomonojono;Initial Catalog=Øyenbryn;Lotsastuff" };
        var database = _serviceProvider.GetKeyedService<IDatabase>(databaseType)!;
        var adminConnectionString = database.GetAdminConnectionString(cfg);
        adminConnectionString.Should().Be("Data source=Monomonojono;Initial Catalog=master;Lotsastuff");
    }

    [Theory]
    [InlineData(grate.PostgreSql.Infrastructure.DatabaseType.Name)]
    public void Uses_ConnectionString_with_master_db_if_adminConnectionString_is_not_set_Database(string databaseType)
    {
        var cfg = new GrateConfiguration()
        { ConnectionString = "Data source=Monomonojono;Database=Øyenbryn;Lotsastuff" };
        var database = _serviceProvider.GetKeyedService<IDatabase>(databaseType)!;
        var adminConnectionString = database.GetAdminConnectionString(cfg);
        adminConnectionString.Should().Be("Data source=Monomonojono;Database=master;Lotsastuff");
    }

    [Fact]
    public void Doesnt_include_comma_in_drop_folder()
    {
        // For bug #40
        var cfg = new GrateConfiguration()
        { ConnectionString = "Data source=localhost,1433;Initial Catalog=Øyenbryn;" };

        var db = new SqlServerDatabase(NullLogger<SqlServerDatabase>.Instance);
        db.InitializeConnections(cfg);
        var dropFolder = GrateMigrator.ChangeDropFolder(cfg, db.ServerName, db.DatabaseName);

        dropFolder.Should().NotContain(",");
    }

}
