using System.IO;
using grate.Configuration;
using grate.unittests.TestInfrastructure;

namespace grate.unittests.Generic.Running_MigrationScripts;

public abstract class MigrationsScriptsBase
{
    protected static DirectoryInfo CreateRandomTempDirectory() => TestConfig.CreateRandomTempDirectory();

    protected void CreateDummySql(DirectoryInfo root, MigrationsFolder? folder, string filename = "1_jalla.sql")
        => CreateDummySql(Wrap(root, folder?.Path), filename);

    protected void WriteSomeOtherSql(DirectoryInfo root, MigrationsFolder? folder, string filename = "1_jalla.sql")
        => WriteSomeOtherSql(Wrap(root,folder?.Path), filename);
        
    protected void CreateDummySql(DirectoryInfo? path, string filename = "1_jalla.sql")
    {
        var dummySql = Context.Sql.SelectVersion;
        WriteSql(path, filename, dummySql);
    }

    protected void WriteSomeOtherSql(DirectoryInfo? path, string filename = "1_jalla.sql")
    {
        var dummySql = Context.Syntax.CurrentDatabase;
        WriteSql(path, filename, dummySql);
    }
    
    protected static void WriteSql(DirectoryInfo root, string path, string filename, string? sql) =>
        TestConfig.WriteContent(Wrap(root, path) , filename, sql);

    protected static void WriteSql(DirectoryInfo? path, string filename, string? sql) =>
        TestConfig.WriteContent(path, filename, sql);

    protected static DirectoryInfo MakeSurePathExists(DirectoryInfo root, MigrationsFolder? folder) 
        => TestConfig.MakeSurePathExists(Wrap(root, folder?.Path));

    protected abstract IGrateTestContext Context { get; }

    protected static DirectoryInfo Wrap(DirectoryInfo root, string? subFolder) =>
        new DirectoryInfo(Path.Combine(root.ToString(), subFolder ?? ""));

}
