using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Oracle.ManagedDataAccess.Client;

namespace grate.unittests.Oracle
{
    [TestFixture]
    public class DockerContainer
    {
        [Test]
        public async Task IsUpAndRunning()
        {
            var db = "master";
            var pw = GrateTestContext.SqlServer.AdminPassword;
            var port = GrateTestContext.SqlServer.Port;

            //var connectionString = $"Data Source=localhost:{port};Initial Catalog={db};User Id=sa;Password={pw};DBA Privilege=SYSDBA";
            //var connectionString = $"user id=scott;password=tiger;data source=//localhost:{port}/sales.us.acme.com";

            var connectionString = $"user id=sa;password={pw};data source=" +
                                   $"(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)" +
                                   $"(HOST=localhost)(PORT={port}))(CONNECT_DATA=" +
                                   $"(SERVICE_NAME={db})))";

            var sql = "SELECT @@VERSION";

            string? res;
            await using (var conn = new OracleConnection(connectionString))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                res = (string?) await cmd.ExecuteScalarAsync();
            }

            res.Should().StartWith("Microsoft SQL Server 2017");

            //TestContext.Progress.WriteLine("Connection string: " + connectionString);
        }
    }
}
