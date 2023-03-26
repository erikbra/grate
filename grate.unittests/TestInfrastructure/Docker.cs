using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace grate.unittests.TestInfrastructure;

public static class Docker
{
    public static async Task<(string containerId, int port)> StartDockerContainer(string serverName, string adminPassword, int? containerPort,  Func<string, string, string> getStartArgs)
    {
        var containerId = await RunDockerCommand(getStartArgs(serverName, adminPassword));
        var findPortArgs = containerPort switch
        {
            { } val => "inspect --format=\"{{(index (index .NetworkSettings.Ports \\\"" + val + "/tcp\\\") 0).HostPort}}\" " +
                       containerId,
            _ =>
                "inspect --format=\"{{range $p, $conf := .NetworkSettings.Ports}} {{(index $conf 0).HostPort}} {{end}}\" " +
                containerId
        };

        //TestContext.Progress.WriteLine("find port: " + findPortArgs);

        var hostPortList = await RunDockerCommand(findPortArgs);
        var hostPort = hostPortList.Split(" ", StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        if (hostPort is null)
        {
            throw new TestInfrastructureSetupException(
                $"Unable to get host port from docker run output '${hostPortList}'");
        }

        return (serverName, int.Parse(hostPort));
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
