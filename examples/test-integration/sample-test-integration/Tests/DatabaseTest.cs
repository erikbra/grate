using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Tests;

[Collection(nameof(SqlServerCollection))] // tell XUnit we need the SqlServer fixture running before this test starts
public sealed class DatabaseTest
{
    [Fact]
    public async Task CheckDatabaseConnectivityAndEnsureMigrationRan()
    {
        var connString = SqlServerFixture.SysAdminConnString;
        await using var conn = new SqlConnection(connString);
        var result = await conn.ExecuteScalarAsync<int>("sample_proc", commandType: CommandType.StoredProcedure);
        Assert.Equal(10, result);
    }
}
