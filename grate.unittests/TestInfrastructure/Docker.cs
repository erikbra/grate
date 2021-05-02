using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace grate.unittests.TestInfrastructure
{
    public static class Docker
    {
        public static async Task<(string containerId, int port)> StartSqlServer(string serverName, string adminPassword)
        {
            var startArgs =
                $"run -d --name {serverName} -e ACCEPT_EULA=Y -e SA_PASSWORD={adminPassword} -e MSSQL_PID=Developer -e MSSQL_COLLATION=Danish_Norwegian_CI_AS -P microsoft/mssql-server-linux:2017-latest";
            
            var containerId = await RunDockerCommand(startArgs);
            var findPortArgs = "inspect --format=\"{{range $p, $conf := .NetworkSettings.Ports}} {{(index $conf 0).HostPort}} {{end}}\" " + containerId;

            //TestContext.Progress.WriteLine("find port: " + findPortArgs);
            
            var hostPort = await RunDockerCommand(findPortArgs);
            //return (containerId, hostPort);
            return (serverName, int.Parse(hostPort));
        }

        public static async Task<(string containerId, int port)> StartOracle(string serverName, string adminPassword)
        {
            var startArgs = 
                $"run -d --name {serverName} -e ORACLE_PWD={adminPassword} -e ORACLE_ALLOW_REMOTE=true -P store/oracle/database-enterprise:12.2.0.1";
            
            var containerId = await RunDockerCommand(startArgs);
            var findPortArgs = "inspect --format=\"{{range $p, $conf := .NetworkSettings.Ports}} {{(index $conf 0).HostPort}} {{end}}\" " + containerId;

            //TestContext.Progress.WriteLine("find port: " + findPortArgs);
            
            var hostPortList = await RunDockerCommand(findPortArgs);
            var hostPort = hostPortList.Split(" ", StringSplitOptions.RemoveEmptyEntries).First();
            //return (containerId, hostPort);
            return (serverName, int.Parse(hostPort));
        }
        
        public static async Task<(string containerId, int port)> StartPostgreSQL(string serverName, string adminPassword)
        {
            var startArgs =
                $"run -d --name {serverName} -e POSTGRES_PASSWORD={adminPassword} -P postgres:latest";
            
            var containerId = await RunDockerCommand(startArgs);
            var findPortArgs = "inspect --format=\"{{range $p, $conf := .NetworkSettings.Ports}} {{(index $conf 0).HostPort}} {{end}}\" " + containerId;

            //TestContext.Progress.WriteLine("find port: " + findPortArgs);
            
            var hostPort = await RunDockerCommand(findPortArgs);
            //return (containerId, hostPort);
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
}