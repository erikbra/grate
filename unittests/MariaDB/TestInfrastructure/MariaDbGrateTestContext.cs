﻿using System.Data;
using grate.Infrastructure;
using grate.MariaDb.Migration;
using grate.Migration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using TestCommon.TestInfrastructure;

namespace MariaDB.TestInfrastructure;

public class MariaDbGrateTestContext : IGrateTestContext
{
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly MariaDbTestContainer _testContainer;
    private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
    public MariaDbGrateTestContext(IServiceProvider serviceProvider, MariaDbTestContainer container)
    {
        ServiceProvider = serviceProvider;
        _testContainer = container;
        DatabaseMigrator = ServiceProvider.GetService<IDatabase>()!;
        Syntax = ServiceProvider.GetService<ISyntax>()!;
        _databaseConnectionFactory = ServiceProvider.GetService<IDatabaseConnectionFactory>()!;

    }

    // public string DockerCommand(string serverName, string adminPassword) =>
    //     $"run -d --name {serverName} -e MYSQL_ROOT_PASSWORD={adminPassword} -P mariadb:10.5.9";

    public string AdminConnectionString => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database=mysql;Uid=root;Pwd={AdminPassword}";
    public string ConnectionString(string database) => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Uid=root;Pwd={AdminPassword}";
    public string UserConnectionString(string database) => $"Server={_testContainer.TestContainer!.Hostname};Port={Port};Database={database};Uid={database};Pwd=mooo1213";

    public IDbConnection GetDbConnection(string connectionString) => _databaseConnectionFactory.GetDbConnection(connectionString);

    public ISyntax Syntax { get; init; }
    public Type DbExceptionType => typeof(MySqlException);

    public string DatabaseType => MariaDbDatabase.Type;
    public bool SupportsTransaction => false;
    // public string DatabaseTypeName => "MariaDB Server";
    // public string MasterDatabase => "mysql";

    public IDatabase DatabaseMigrator { get; init; }

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT VERSION()",
        SleepTwoSeconds = "SELECT SLEEP(2);",
        CreateUser = "CREATE USER '{0}'@'%' IDENTIFIED BY '{1}';",
        GrantAccess = "GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, INDEX, DROP, ALTER, CREATE TEMPORARY TABLES, LOCK TABLES ON {0}.* TO '{1}'@'%';FLUSH PRIVILEGES;"
    };


    public string ExpectedVersionPrefix => "10.10.7-MariaDB";
    public bool SupportsCreateDatabase => true;
}
