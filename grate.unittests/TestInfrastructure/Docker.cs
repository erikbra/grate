using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace grate.unittests.TestInfrastructure;

public static class Docker
{
    public static async Task<(string containerId, int port)> StartDockerContainer(string serverName, string adminPassword, Func<string, string, string> getStartArgs)
    {
        var containerId = await RunDockerCommand(getStartArgs(serverName, adminPassword));
        var findPortArgs = "inspect --format=\"{{range $p, $conf := .NetworkSettings.Ports}} {{(index $conf 0).HostPort}} {{end}}\" " + containerId;

        //TestContext.Progress.WriteLine("find port: " + findPortArgs);

        var hostPortList = await RunDockerCommand(findPortArgs);
        var hostPort = hostPortList.Split(" ", StringSplitOptions.RemoveEmptyEntries).First();
        return (serverName, int.Parse(hostPort));
    }

    public static async Task<(string containerId, int port)> StartSqlServer(string serverName, string adminPassword)
    {
        var startArgs =
            $"run -d --name {serverName} -e ACCEPT_EULA=Y -e SA_PASSWORD={adminPassword} -e MSSQL_PID=Developer -e MSSQL_COLLATION=Danish_Norwegian_CI_AS -P microsoft/mssql-server-linux:2017-latest";

        return await StartDockerContainer(serverName, adminPassword, (_, _) => startArgs);
    }

    public static async Task<(string containerId, int port)> StartOracle(string serverName, string adminPassword)
    {
        var startArgs =
            $"run -d --name {serverName} -e ORACLE_PWD={adminPassword} -e ORACLE_ALLOW_REMOTE=true -P store/oracle/database-enterprise:12.2.0.1";

        return await StartDockerContainer(serverName, adminPassword, (_, _) => startArgs);
    }

    // ReSharper disable once InconsistentNaming
    public static async Task<(string containerId, int port)> StartPostgreSQL(string serverName, string adminPassword)
    {
        var startArgs =
            $"run -d --name {serverName} -e POSTGRES_PASSWORD={adminPassword} -P postgres:latest";

        return await StartDockerContainer(serverName, adminPassword, (_, _) => startArgs);
    }


    public static async Task<string> Delete(string container)
    {
        await RunDockerCommand($"stop {container}");
        return await RunDockerCommand($"rm {container}");
    }


    private static async Task<string> RunDockerCommand(string args)
    {
        //TestContext.Progress.WriteLine("args: " + args);

        var proc = new Process
        {
            StartInfo = new ProcessStartInfo("docker", args)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true
            }
        };

        proc.Start();

        string errors = await proc.StandardError.ReadToEndAsync();
        if (!string.IsNullOrWhiteSpace(errors))
        {
            await TestContext.Error.WriteLineAsync("Error: " + errors);
        }

        string output = (await proc.StandardOutput.ReadToEndAsync()).Trim();
        //await TestContext.Progress.WriteLineAsync("Output: " + output);

        await proc.WaitForExitAsync();

        return output;
    }

}
