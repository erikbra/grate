using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace grate.unittests.TestInfrastructure;

class SqlServerGrateTestContext : TestContextBase, IGrateTestContext, IDockerTestContext
{
    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }

    public string DockerCommand(string serverName, string adminPassword) =>
        $"run -d --name {serverName} -e ACCEPT_EULA=Y -e SA_PASSWORD={adminPassword} -e MSSQL_PID=Developer -e MSSQL_COLLATION=Danish_Norwegian_CI_AS -P mcr.microsoft.com/mssql/server:latest";

    public string AdminConnectionString => $"Data Source=localhost,{Port};Initial Catalog=master;User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";
    public string ConnectionString(string database) => $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public DbConnection GetDbConnection(string connectionString) => new SqlConnection(connectionString);

    public ISyntax Syntax => new SqlServerSyntax();
    public Type DbExceptionType => typeof(SqlException);

    public DatabaseType DatabaseType => DatabaseType.sqlserver;
    public bool SupportsTransaction => true;
    public string DatabaseTypeName => "SQL server";
    public string MasterDatabase => "master";

    public IDatabase DatabaseMigrator => new SqlServerDatabase(TestConfig.LogFactory.CreateLogger<SqlServerDatabase>());

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT @@VERSION",
    };

    public string ExpectedVersionPrefix => "Microsoft SQL Server 2019";
    public bool SupportsCreateDatabase => true;
}
