﻿using System;
using System.IO;
using grate.Configuration;
using grate.unittests.TestInfrastructure;

namespace grate.unittests.Generic.Running_MigrationScripts
{
    public abstract class MigrationsScriptsBase
    {
        protected static DirectoryInfo CreateRandomTempDirectory()
        {

            // I keep seeing the same temp di name in repeat test runs, and the dir has leftover scripts sitting in it.
            // Trying to get a clean folder each time we ask for it

            var dummyFile = Path.GetRandomFileName();
            File.Delete(dummyFile);

            var scriptsDir = Directory.CreateDirectory(dummyFile);
            return scriptsDir;
        }

        protected void CreateDummySql(MigrationsFolder? folder, string filename = "1_jalla.sql")
        {
            var dummySql = Context.Sql.SelectVersion;
            var path = MakeSurePathExists(folder);
            WriteSql(path, filename, dummySql);
        }

        protected void WriteSomeOtherSql(MigrationsFolder? folder, string filename = "1_jalla.sql")
        {
            var dummySql = Context.Syntax.CurrentDatabase;
            var path = MakeSurePathExists(folder);
            WriteSql(path, filename, dummySql);
        }

        protected static void WriteSql(DirectoryInfo path, string filename, string? sql)
        {
            if (!path.Exists)
            {
                path.Create();
            }
            File.WriteAllText(Path.Combine(path.ToString(), filename), sql);
        }

        protected static DirectoryInfo MakeSurePathExists(MigrationsFolder? folder)
        {
            var path = folder?.Path ?? throw new ArgumentException(nameof(folder.Path));

            if (!path.Exists)
            {
                path.Create();
            }

            return path;
        }

        protected abstract IGrateTestContext Context { get; }
    }
}
