using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Generic;

[TestFixture]
public abstract class GenericDockerContainer
{
    protected abstract IGrateTestContext Context { get; }
        
    [Test]
    public async Task IsUpAndRunning()
    {
        string? res;
        await using (var conn = Context.CreateAdminDbConnection()) 
        {
            await conn.OpenAsync();

            var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = Context.Sql.SelectVersion;

            res = (string?) await cmd.ExecuteScalarAsync();
        }

        res.Should().StartWith(Context.ExpectedVersionPrefix);
    }
}