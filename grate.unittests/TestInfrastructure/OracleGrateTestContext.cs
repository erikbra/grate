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

    public string DockerCommand(string serverName, string adminPassword) =>
        $"run -d --name {serverName} -e ORACLE_ENABLE_XDB=true -P oracleinanutshell/oracle-xe-11g:latest";

    public string AdminConnectionString => $@"Data Source=localhost:{Port}/XE;User ID=SYSTEM;Password=oracle";
    public string ConnectionString(string database) => $@"Data Source=localhost:{Port}/XE;User ID={database.ToUpper()};Password=oracle";

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

    public string ExpectedVersionPrefix => "Oracle Database 11g Express Edition Release 11.2.0.2.0 - 64bit Production";
    public bool SupportsCreateDatabase => true;
    public bool DatabaseNamingCaseSensitive => false;
}
