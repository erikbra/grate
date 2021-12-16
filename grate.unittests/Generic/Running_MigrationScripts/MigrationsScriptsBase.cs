using System.IO;
using grate.Configuration;
using grate.unittests.TestInfrastructure;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    public abstract class MigrationsScriptsBase
    {
        protected static DirectoryInfo CreateRandomTempDirectory() => TestConfig.CreateRandomTempDirectory();

        protected void CreateDummySql(MigrationsFolder? folder, string filename = "1_jalla.sql")
            => CreateDummySql(folder?.Path, filename);

        protected void WriteSomeOtherSql(MigrationsFolder? folder, string filename = "1_jalla.sql")
            => WriteSomeOtherSql(folder?.Path, filename);
        
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

        protected static void WriteSql(DirectoryInfo? path, string filename, string? sql) =>
            TestConfig.WriteContent(path, filename, sql);

        protected static DirectoryInfo MakeSurePathExists(MigrationsFolder? folder) => TestConfig.MakeSurePathExists(folder?.Path);

    protected abstract IGrateTestContext Context { get; }
}
