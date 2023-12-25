using System.Data;
using System.Runtime.InteropServices;
using grate.Infrastructure;
using grate.Migration;
using grate.SqlServer.Migration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.TestInfrastructure;
using static System.Runtime.InteropServices.Architecture;

namespace SqlServerCaseSensitive.TestInfrastructure;

class SqlServerGrateTestContext : IGrateTestContext
{
    public IServiceProvider ServiceProvider { get; private set; }
    private readonly SqlServerTestContainer _testContainer;

    private readonly IDatabaseConnectionFactory _databaseConnectionFactory;
    public SqlServerGrateTestContext(string serverCollation, IServiceProvider serviceProvider, SqlServerTestContainer container)
    {
        ServiceProvider = serviceProvider;
        _testContainer = container;
        ServerCollation = serverCollation;
        DatabaseMigrator = ServiceProvider.GetService<IDatabase>()!;
        Syntax = ServiceProvider.GetService<ISyntax>()!;
        _databaseConnectionFactory = ServiceProvider.GetService<IDatabaseConnectionFactory>()!;
    }
    public SqlServerGrateTestContext(IServiceProvider serviceProvider, SqlServerTestContainer container) : this("Danish_Norwegian_CI_AS", serviceProvider, container)
    {
    }
    public string AdminPassword => _testContainer.AdminPassword;
    public int? Port => _testContainer.TestContainer!.GetMappedPublicPort(_testContainer.Port);

    public string AdminConnectionString => $"Data Source=localhost,{Port};Initial Catalog=master;User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";
    public string ConnectionString(string database) => $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";
    public string UserConnectionString(string database) => $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public IDbConnection GetDbConnection(string connectionString) => _databaseConnectionFactory.GetDbConnection(connectionString);

    public ISyntax Syntax { get; init; }
    public Type DbExceptionType => typeof(SqlException);

    public string DatabaseType => SqlServerDatabase.Type;
    public bool SupportsTransaction => true;
    // public string DatabaseTypeName => "SQL server";
    // public string MasterDatabase => "master";

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
