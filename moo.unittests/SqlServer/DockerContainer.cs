using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using NUnit.Framework;

namespace moo.unittests.SqlServer
{
    [TestFixture]
    public class DockerContainer
    {
        [Test]
        public async Task IsUpAndRunning()
        {
            var db = "master";
            var pw = MooTestContext.SqlServer.AdminPassword;
            var port = MooTestContext.SqlServer.Port;

            var connectionString = $"Data Source=localhost,{port};Initial Catalog={db};User Id=sa;Password={pw}";

            var sql = "SELECT @@VERSION";

            string? res;
            await using (var conn = new SqlConnection(connectionString))
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
