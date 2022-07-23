using System;
using System.Data.Common;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace grate.unittests.TestInfrastructure;

class MariaDbGrateTestContext : TestContextBase, IGrateTestContext, IDockerTestContext
{
    public MariaDbGrateTestContext(bool databaseNamingCaseSensitive)
    {
        DatabaseNamingCaseSensitive = databaseNamingCaseSensitive;
    }

    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }

    public string DockerCommand(string serverName, string adminPassword) =>
        DatabaseNamingCaseSensitive
            ? $"run -d --name {serverName} -e MYSQL_ROOT_PASSWORD={adminPassword} -P mariadb:10.5.9"
            : $"run -d --name {serverName} -e MYSQL_ROOT_PASSWORD={adminPassword} -P mariadb:10.5.9 --lower_case_table_names=1";

    public string AdminConnectionString => $"Server=localhost;Port={Port};Database=mysql;Uid=root;Pwd={AdminPassword}";
    public string ConnectionString(string database) => $"Server=localhost;Port={Port};Database={database};Uid=root;Pwd={AdminPassword}";

    public DbConnection GetDbConnection(string connectionString) => new MySqlConnection(connectionString);

    public ISyntax Syntax => new MariaDbSyntax();
    public Type DbExceptionType => typeof(MySqlException);

    public DatabaseType DatabaseType => DatabaseType.mariadb;
    public bool SupportsTransaction => false;
    public string DatabaseTypeName => "MariaDB Server";
    public string MasterDatabase => "mysql";

    public IDatabase DatabaseMigrator => new MariaDbDatabase(TestConfig.LogFactory.CreateLogger<MariaDbDatabase>());

    public SqlStatements Sql => new()
    {
        SelectVersion = "SELECT VERSION()",
        SleepTwoSeconds = "SELECT SLEEP(2);"
    };

    public string ExpectedVersionPrefix => "10.5.9-MariaDB";
    public bool SupportsCreateDatabase => true;
    public bool DatabaseNamingCaseSensitive { get; }
}
