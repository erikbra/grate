using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class With_batch_separator: Generic.Running_MigrationScripts.MigrationsScriptsBase
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
    
    [Test()]
    public async Task Separates_multiple_statements()
    {
        var db = TestConfig.RandomDatabase();

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        GrateMigrator? migrator;

        
        var path = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Up]!.Path));
        const string filename = "multiple_statements.sql";
        
        const string fileContent = @"
create table table_one (
   col  number
);
/

create table table_two (
   col number
)
";
        
        WriteSql(path, filename, fileContent);

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(2);
    }

}
