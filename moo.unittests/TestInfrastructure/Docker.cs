using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;

namespace moo.unittests.TestInfrastructure
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

        public static async Task<string> DeleteSqlServer(string serverName)
        {
            await RunDockerCommand($"stop {serverName}");
            return await RunDockerCommand($"rm {serverName}");
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