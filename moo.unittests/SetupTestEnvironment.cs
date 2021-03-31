using System;
using System.Threading.Tasks;
using moo.unittests;
using moo.unittests.Infrastructure;
using NUnit.Framework;

[SetUpFixture]
public class SetupTestEnvironment
{
    private string _serverName;
    private string _containerId;
    
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "1234567890";

    private const string ServerNameAllowedChars = "abcdefghijklmnopqrstuvwxyz";

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        var random = new Random();

        _serverName = "moo-sqlserver-" + random.GetString(10, ServerNameAllowedChars);
        var password = 
            random.GetString(10, UpperCase) + 
            random.GetString(10, LowerCase) +
            random.GetString(10, Digits);
        
        //await TestContext.Out.WriteAsync("Starting SQL server docker container: ");
        int port;
        (_containerId, port) = await Docker.StartSqlServer(_serverName, password);

        MooTestContext.SqlServer.AdminPassword = password;
        MooTestContext.SqlServer.Port = port;
        
        await TestContext.Progress.WriteAsync("Started SQL server docker container: " + _containerId);
        await TestContext.Progress.WriteAsync("Listening on port: " + port);
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        //await TestContext.Progress.WriteAsync("Removing SQL server docker container: ");
        
        var containerId = await Docker.DeleteSqlServer(_containerId);
        await TestContext.Progress.WriteAsync("Removed SQL server docker container: " + containerId);
    }
}