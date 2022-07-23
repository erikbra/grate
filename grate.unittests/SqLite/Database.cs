using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using grate.unittests.TestInfrastructure;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace grate.unittests.Sqlite;

[TestFixture]
[Category("Sqlite")]
public class Database: Generic.GenericDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;

    protected override async Task CreateDatabase(string db)
    {
        try
        {
            await using var conn = new SqliteConnection(Context.ConnectionString(db));
            conn.Open();
            await using var cmd = conn.CreateCommand();
            var sql = "CREATE TABLE dummy(name VARCHAR(1))";
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync();
        }
        catch (DbException) { }
    }

    protected override async Task<IEnumerable<string>> GetDatabases() 
    {
        var dbFiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.db");
        IEnumerable<string> dbNames = dbFiles
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name is not null)
            .Cast<string>();

        return await ValueTask.FromResult(dbNames);
    }

    protected override bool ThrowOnMissingDatabase => false;
}
