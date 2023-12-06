using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using grate.Configuration;
using grate.Infrastructure;
using grate.Migration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
using static System.Runtime.InteropServices.Architecture;

namespace SqlServer.TestInfrastructure;

class SqlServerGrateTestContext : TestContextBase, IGrateTestContext, IDockerTestContext
{


    public SqlServerGrateTestContext(string serverCollation) => ServerCollation = serverCollation;

    public SqlServerGrateTestContext() : this("Danish_Norwegian_CI_AS")
    {
    }

    public string AdminPassword { get; set; } = default!;
    public int? Port { get; set; }
    public override int? ContainerPort => 1433;

    // on arm64 (M1), the standard mssql/server image is not available
    public string? DockerImage => RuntimeInformation.ProcessArchitecture switch
    {
        Arm64 => "mcr.microsoft.com/azure-sql-edge:latest",
        X64 => "mcr.microsoft.com/mssql/server:2019-latest",
        var other => throw new PlatformNotSupportedException("Unsupported platform for running tests: " + other)
    };

    // public string DockerCommand(string serverName, string adminPassword) =>
    //     $"run -d --name {serverName} -e ACCEPT_EULA=Y -e SA_PASSWORD={adminPassword} -e MSSQL_PID=Developer -e MSSQL_COLLATION={ServerCollation} -P {DockerImage}";

    public string AdminConnectionString =>
        $"Data Source=localhost,{Port};Initial Catalog=master;User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public string ConnectionString(string database) =>
        $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

    public string UserConnectionString(string database) =>
        $"Data Source=localhost,{Port};Initial Catalog={database};User Id=sa;Password={AdminPassword};Encrypt=false;Pooling=false";

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
        SleepTwoSeconds = "WAITFOR DELAY '00:00:02'"
    };

    public string ExpectedVersionPrefix => RuntimeInformation.ProcessArchitecture switch
    {
        Arm64 => "Microsoft Azure SQL Edge Developer",
        X64 => "Microsoft SQL Server 2019",
        var other => throw new PlatformNotSupportedException("Unsupported platform for running tests: " + other)
    };

    public bool SupportsCreateDatabase => true;

    public string ServerCollation { get; }

    public ContainerBuilder AddEnvironmentVariables(ContainerBuilder builder)
    {
        return builder.WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("SQLCMDDBNAME", "master")
            .WithEnvironment("SQLCMDUSER", "sa")
            .WithEnvironment("SQLCMDPASSWORD", AdminPassword)
            .WithEnvironment("MSSQL_SA_PASSWORD", AdminPassword)
            .WithEnvironment("MSSQL_PID", "Developer")
            .WithEnvironment("MSSQL_COLLATION", ServerCollation);
    }

    public IWaitUntil WaitStrategy => new WaitUntil();

    private sealed class WaitUntil : IWaitUntil
    {
        private readonly string[] _command =
        {
            "/opt/mssql-tools/bin/sqlcmd", "-Q", "SELECT 1;",
        };

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var execResult = await container.ExecAsync(_command)
                .ConfigureAwait(false);

            return 0L.Equals(execResult.ExitCode);
        }
    }
}
