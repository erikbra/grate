using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace grate.unittests.TestInfrastructure;

internal class OracleGrateTestContext : TestContextBase, IGrateTestContext, IDockerTestContext
{
    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }
    public override int? ContainerPort => 1521;

    public string DockerCommand(string serverName, string adminPassword) =>
        $"run -d --name {serverName} -p 1521 -e ORACLE_ENABLE_XDB=true -e ORACLE_PWD={adminPassword} -P container-registry.oracle.com/database/express:21.3.0-xe";


    public string AdminConnectionString => $@"Data Source=localhost:{Port}/XEPDB1;User ID=system;Password={AdminPassword};Pooling=False";
    public string ConnectionString(string database) => $@"Data Source=localhost:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";
    public string UserConnectionString(string database) => $@"Data Source=localhost:{Port}/XEPDB1;User ID={database.ToUpper()};Password={AdminPassword};Pooling=False";

    public DbConnection GetDbConnection(string connectionString) => new OracleConnection(connectionString);

    public ISyntax Syntax => new OracleSyntax();
    public Type DbExceptionType => typeof(OracleException);

    public DatabaseType DatabaseType => DatabaseType.oracle;
    public bool SupportsTransaction => false;
    public string DatabaseTypeName => "Oracle";
    public string MasterDatabase => "oracle";

    public IDatabase DatabaseMigrator => new OracleDatabase(TestConfig.LogFactory.CreateLogger<OracleDatabase>());

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT * FROM v$version WHERE banner LIKE 'Oracle%'",
        SleepTwoSeconds = "sys.dbms_session.sleep(2);"
    };

    public string ExpectedVersionPrefix => "Oracle Database 21c Express Edition Release 21.0.0.0.0 - Production";
    public bool SupportsCreateDatabase => true;
}
