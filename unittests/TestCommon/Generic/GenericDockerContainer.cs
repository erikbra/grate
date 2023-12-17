using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using TestCommon.TestInfrastructure;

namespace TestCommon.Generic;

public abstract class GenericDockerContainer
{
    protected abstract IGrateTestContext Context { get; }

    [Fact]
    public async Task Is_up_and_running()
    {
        string? res;
        await using (var conn = Context.CreateAdminDbConnection())
        {
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Context.Sql.SelectVersion;

            res = (string?)await cmd.ExecuteScalarAsync();
        }

        res.Should().StartWith(Context.ExpectedVersionPrefix);
    }
}
