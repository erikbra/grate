using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using Oracle.TestInfrastructure;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace Oracle.Oracle_specific;

public class Issue283_Ora00922: MigrationsScriptsBase
{
    private const string problematicSql = @"
CREATE TABLE actor (
actor_id numeric NOT NULL ,
first_name VARCHAR(45) NOT NULL,
last_name VARCHAR(45) NOT NULL,
last_update DATE NOT NULL,
CONSTRAINT pk_actor PRIMARY KEY (actor_id)
);";

        
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
    
    [Test]
    public async Task Are_not_run_more_than_once_when_unchanged()
    {
        var db = TestConfig.RandomDatabase();
        GrateMigrator? migrator;
            
        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();

        const string scriptName = "crate-table-actor.sql";

        var upFolder = Wrap(parent, knownFolders[Up]!.Path);
        WriteSql(upFolder, scriptName, problematicSql);
       
        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        string[] scripts;
        string sql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";
            
        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.Should().HaveCount(1);
        scripts.First().Should().Be(scriptName);
    }
    
}
