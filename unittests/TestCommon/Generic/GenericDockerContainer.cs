using Dapper;
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
        using (var conn = Context.CreateAdminDbConnection())
        {
            conn.Open();

            // var cmd = conn.CreateCommand();
            // cmd.CommandType = CommandType.Text;
            var commandText = Context.Sql.SelectVersion;

            res = (string?)await conn.ExecuteScalarAsync(commandText);
        }

        res.Should().StartWith(Context.ExpectedVersionPrefix);
    }
}
