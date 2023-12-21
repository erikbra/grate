﻿using System.Data.Common;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;

namespace SqlServer.TestInfrastructure;

class SqlServerGrateTestContext : IGrateTestContext
{
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly SqlServerTestContainer _testContainer;
    public SqlServerGrateTestContext(string serverCollation, IServiceProvider serviceProvider, SqlServerTestContainer container)
    {
        ServiceProvider = serviceProvider;
        _testContainer = container;
        ServerCollation = serverCollation;
        DatabaseMigrator = ServiceProvider.GetService<IDatabase>()!;
        Syntax = ServiceProvider.GetService<ISyntax>()!;
    }
    public SqlServerGrateTestContext(IServiceProvider serviceProvider, SqlServerTestContainer container) : this("Danish_Norwegian_CI_AS", serviceProvider, container)
    {
    }
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);

    //private int? ContainerPort => 1433;


    // public string DockerCommand(string serverName, string adminPassword) =>
    //     $"run -d --name {serverName} -e ACCEPT_EULA=Y -e SA_PASSWORD={adminPassword} -e MSSQL_PID=Developer -e MSSQL_COLLATION={ServerCollation} -P {DockerImage}";

    public string AdminConnectionString =>
        $"Data Source=localhost,{Port};Initial Catalog=master;User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public string ConnectionString(string database) =>
        $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public string UserConnectionString(string database) =>
        $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public DbConnection GetDbConnection(string connectionString) => new SqlConnection(connectionString);



    public ISyntax Syntax { get; init; }
    public Type DbExceptionType => typeof(SqlException);

    public string DatabaseType => "sqlserver";
    public bool SupportsTransaction => true;
    public string DatabaseTypeName => "SQL server";
    public string MasterDatabase => "master";

    public IDatabase DatabaseMigrator { get; init; }

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT @@VERSION",
        SleepTwoSeconds = "WAITFOR DELAY '00:00:02'"
    };

    public string ExpectedVersionPrefix => "Microsoft SQL Server 2019";

    public bool SupportsCreateDatabase => true;

    public string ServerCollation { get; }
}
