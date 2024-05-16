using Basic_tests.Infrastructure;
using FluentAssertions;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Basic_tests;

// ReSharper disable once InconsistentNaming
public class GrateMigrator_
{
    private readonly IDatabase _database = Substitute.For<IDatabase>();
    private readonly GrateConfiguration? _config = new();

    public GrateMigrator_()
    {
        _database.Clone().Returns(_database);
    }

    [Fact]
    public void Setting_the_config_does_not_change_the_original()
    {
        var config = new GrateConfiguration() { ConnectionString = "Server=server1" };
        var dbMigrator = new DbMigrator(_database, null!, null!, config);

        var grateMigrator = new GrateMigrator(new MockGrateLoggerFactory(), dbMigrator);

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

        var grateMigrator = new GrateMigrator(new MockGrateLoggerFactory(), dbMigrator);

        grateMigrator.Database.DatabaseName.Should().Be("server1");

        var changedDatabase = Substitute.For<IDatabase>();
        changedDatabase.DatabaseName.Returns("server2");

        var changedMigrator = grateMigrator.WithDatabase(changedDatabase) as GrateMigrator;

        grateMigrator.Database.DatabaseName.Should().Be("server1");
        changedMigrator!.Database.DatabaseName.Should().Be("server2");
    }
    
    [Theory]
    [MemberData(nameof(Environments))]
    public void Logger_has_the_correct_LogCategory(GrateEnvironment environment, string logCategory)
    {
        var config = new GrateConfiguration() { Environment = environment };
        var dbMigrator = new DbMigrator(_database, null!, null!, config);
        
        var loggerFactory = Substitute.For<ILoggerFactory>();
        _ = new GrateMigrator(loggerFactory, dbMigrator);

        loggerFactory.Received().CreateLogger(logCategory);
    }
    
    public static TheoryData<GrateEnvironment, string> Environments() => new()
    {
        { GrateEnvironment.Internal, "Grate.Migration.Internal" },
        { GrateEnvironment.InternalBootstrap, "Grate.Migration.Internal" },
        { new GrateEnvironment("Development"), "Grate.Migration" },
        { new GrateEnvironment("Test"), "Grate.Migration" },
        { new GrateEnvironment("Production"), "Grate.Migration" },
    };

}
