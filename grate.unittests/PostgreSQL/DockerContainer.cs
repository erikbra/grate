using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Npgsql;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL
{
    [TestFixture]
    public class DockerContainer
    {
        [Test]
        public async Task IsUpAndRunning()
        {
            var db = "postgres";
            var pw = GrateTestContext.PostgreSql.AdminPassword;
            var port = GrateTestContext.PostgreSql.Port;

            var connectionString = $"Host=localhost;Port={port};Database={db};Username=postgres;Password={pw}";

            var sql = "SELECT version()";

            string? res;
            await using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                res = (string?) await cmd.ExecuteScalarAsync();
            }

            res.Should().StartWith("PostgreSQL 13.");

            //TestContext.Progress.WriteLine("Connection string: " + connectionString);
        }
    }
}
