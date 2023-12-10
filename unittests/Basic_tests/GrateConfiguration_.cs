using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using Microsoft.Extensions.Logging.Abstractions;

namespace Basic_tests;

// ReSharper disable once InconsistentNaming
public class GrateConfiguration_
{
    [Fact]
    public void Uses_ConnectionString_with_master_db_if_adminConnectionString_is_not_set_Initial_Catalog()
    {
        var cfg = new GrateConfiguration()
        { ConnectionString = "Data source=Monomonojono;Initial Catalog=Øyenbryn;Lotsastuff" };

        cfg.AdminConnectionString.Should().Be("Data source=Monomonojono;Initial Catalog=master;Lotsastuff");
    }

    [Fact]
    public void Uses_ConnectionString_with_master_db_if_adminConnectionString_is_not_set_Database()
    {
        var cfg = new GrateConfiguration()
        { ConnectionString = "Data source=Monomonojono;Database=Øyenbryn;Lotsastuff" };

        cfg.AdminConnectionString.Should().Be("Data source=Monomonojono;Database=master;Lotsastuff");
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
