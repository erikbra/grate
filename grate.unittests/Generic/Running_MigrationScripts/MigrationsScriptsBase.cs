using System;
using System.IO;
using System.Text;
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
    
    protected void CreateLargeDummySql(DirectoryInfo? path, int size = 8192, string filename = "1_very_large_file.sql")
    {
        var longComment = CreateLongComment(size);
        
        var dummySql = longComment + Environment.NewLine + Context.Sql.SelectVersion;
        WriteSql(path, filename, dummySql);
    }

    private string CreateLongComment(int size)
    {
        const int lineLen = 80;
        var numLines = size / lineLen;
        var rest = size - (lineLen * numLines);

        var filler = new string('A', Math.Min(lineLen, size));

        var builder = new StringBuilder((Context.Sql.LineComment.Length + 1 + lineLen + 2) * numLines + rest);
        for (var i = 0; i < numLines; i++)
        {
            builder.Append(Context.Sql.LineComment);
            builder.Append(' ');
            builder.AppendLine(filler);
        }
        if (rest > 0)
        {
            builder.Append(Context.Sql.LineComment);
            builder.Append(' ');
            builder.AppendLine(new string('A', rest));
        }

        return builder.ToString();
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
