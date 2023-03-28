using System;
using System.IO;
using System.Threading.Tasks;
using grate.Configuration;
using grate.unittests.Generic.Running_MigrationScripts;
using grate.unittests.TestInfrastructure;
using NUnit.Framework;
using static grate.Configuration.KnownFolderKeys;

namespace grate.unittests.SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
public class RunAfterCreateDatabase_ : MigrationsScriptsBase
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;

    private const string Bug232Sql = @"
ALTER DATABASE {{DatabaseName}} SET ALLOW_SNAPSHOT_ISOLATION ON;
ALTER DATABASE {{DatabaseName}} SET READ_COMMITTED_SNAPSHOT ON;";


    private const string Bug232Sql_1 = @"
ALTER DATABASE {{DatabaseName}} SET ALLOW_SNAPSHOT_ISOLATION ON;";

    private const string Bug232Sql_2 = @"
ALTER DATABASE {{DatabaseName}} SET READ_COMMITTED_SNAPSHOT ON;";

    [Test]
    public async Task Bug232_Timeout_14_Regression()
    {
        // V1.4 regressed something, trying to repro

        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default();
        var path = new DirectoryInfo(Path.Combine(parent.ToString(),
            knownFolders[AlterDatabase]?.Path ?? throw new Exception("Config Fail")));

        //WriteSql(path, "bothscripts.sql", Bug232Sql);
        
        WriteSql(path, "script1.sql", Bug232Sql_1);
        WriteSql(path, "script2.sql", Bug232Sql_2);

        await using (var migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        // Now drop it and do it again
        var config = Context.GetConfiguration(db, parent, knownFolders) with { Drop = true };
        
        await using (var migrator = Context.GetMigrator(config))
        {
            await migrator.Migrate();
        }
    }
}
