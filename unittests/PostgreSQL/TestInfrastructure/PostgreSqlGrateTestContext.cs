﻿using System.Data;
using grate.Infrastructure;
using grate.Migration;
using grate.PostgreSql.Migration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.TestInfrastructure;

public class PostgreSqlGrateTestContext : IGrateTestContext
{
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly PostgreSqlTestContainer _testContainer;
    private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
    public PostgreSqlGrateTestContext(IServiceProvider serviceProvider, PostgreSqlTestContainer container)
    {
        ServiceProvider = serviceProvider;
        _testContainer = container;
        DatabaseMigrator = serviceProvider.GetRequiredService<IDatabase>();
        Syntax = serviceProvider.GetRequiredService<ISyntax>();
        _databaseConnectionFactory = serviceProvider.GetRequiredService<IDatabaseConnectionFactory>();

    }
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);


    public string AdminConnectionString =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database=postgres;Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public string ConnectionString(string database) =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public string UserConnectionString(string database) =>
        $"Host={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Username=postgres;Password={AdminPassword};Include Error Detail=true;Pooling=false";

    public IDbConnection GetDbConnection(string connectionString) => _databaseConnectionFactory.GetDbConnection(connectionString);

    public ISyntax Syntax { get; init; }
    public Type DbExceptionType => typeof(PostgresException);

    public string DatabaseType => PostgreSqlDatabase.Type;
    public bool SupportsTransaction => true;
    // public string DatabaseTypeName => "PostgreSQL";
    // public string MasterDatabase => "postgres";

    public IDatabase DatabaseMigrator { get; init; }

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT version()",
        SleepTwoSeconds = "SELECT pg_sleep(2);"
    };

    public string ExpectedVersionPrefix => "PostgreSQL 16.";
    public bool SupportsCreateDatabase => true;

}
