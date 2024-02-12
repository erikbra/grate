﻿using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Basic_tests;

// ReSharper disable once InconsistentNaming
public class GrateMigrator_
{
    private IDatabase _database = Substitute.For<IDatabase>();
    private GrateConfiguration? _config = new GrateConfiguration();

    [Fact]
    public void Setting_the_config_does_not_change_the_original()
    {
        var config = new GrateConfiguration() { ConnectionString = "Server=server1" };
        var dbMigrator = new DbMigrator(_database, null!, null!, config);
        
        var grateMigrator = new GrateMigrator(null!, dbMigrator);
        
        grateMigrator.Configuration.Should().BeEquivalentTo(config);
        
        var changedConfig = config with { ConnectionString = "Server=server2" };
        var changedMigrator = grateMigrator.WithConfiguration(changedConfig);
        
        grateMigrator.Configuration.ConnectionString.Should().Be("Server=server1");
        changedMigrator.Configuration.ConnectionString.Should().Be("Server=server2");
    }
    
    [Fact]
    public void Setting_the_Database_does_not_change_the_original()
    {
        _database.DatabaseName.Returns("server1");
        var dbMigrator = new DbMigrator(_database, null!, null!, _config);
        
        var grateMigrator = new GrateMigrator(null!, dbMigrator);
        
        grateMigrator.Database.DatabaseName.Should().Be("server1");
        
        var changedDatabase = Substitute.For<IDatabase>();
        changedDatabase.DatabaseName.Returns("server2");
        
        var changedMigrator = grateMigrator.WithDatabase(changedDatabase) as GrateMigrator;

        grateMigrator.Database.DatabaseName.Should().Be("server1");
        changedMigrator!.Database.DatabaseName.Should().Be("server2");
    }
    
}
