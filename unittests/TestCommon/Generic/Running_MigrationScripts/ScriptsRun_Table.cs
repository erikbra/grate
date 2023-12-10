using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Migration;
using TestCommon.TestInfrastructure;
using static grate.Configuration.KnownFolderKeys;

namespace TestCommon.Generic.Running_MigrationScripts;

public abstract class ScriptsRun_Table : MigrationsScriptsBase
{
    [Fact]
    public async Task Includes_the_folder_name_in_the_script_name_if_subfolders()
    {
        var db = TestConfig.RandomDatabase();

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        GrateMigrator? migrator;

        var folder = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Up]!.Path, "sub", "folder", "long", "way"));

        string filename = "any_filename.sql";

        CreateDummySql(folder, filename);
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

        var expectedName = $"sub/folder/long/way/{filename}";
        scripts.First().Should().Be(expectedName);
    }

    [Fact]
    public async Task Does_not_include_the_folder_name_in_the_script_name_if_no_subfolders()
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);

        GrateMigrator? migrator;

        string filename = "any_filename.sql";

        CreateDummySql(parent, knownFolders[Up], filename);

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

        scripts.First().Should().Be(filename);
    }

    // ReSharper disable InconsistentNaming
    // ReSharper disable once ClassNeverInstantiated.Local
    record Result(string script_name, string text_of_script);
    // ReSharper restore InconsistentNaming

    [Fact]
    public async Task Does_not_overwrite_scripts_from_different_folders_with_last_content()
    {
        var db = TestConfig.RandomDatabase();

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        GrateMigrator? migrator;

        string filename = "any_filename.sql";
        var folder1 = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Up]!.Path, "dub", "folder", "long", "way"));
        var folder2 = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Up]!.Path, "sub", "dolder", "gong", "way"));

        CreateDummySql(folder1, filename);
        WriteSomeOtherSql(folder2, filename);

        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }


        Result[] scripts;
        string sql = $"SELECT script_name, text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<Result>(sql)).ToArray();
        }

        Assert.Multiple(() =>
        {
            scripts.Should().HaveCount(2);
            var first = scripts.First();
            var second = scripts.Last();

            first.script_name.Should().Be($"dub/folder/long/way/{filename}");
            first.text_of_script.Should().Be(Context.Sql.SelectVersion);

            second.script_name.Should().Be($"sub/dolder/gong/way/{filename}");
            second.text_of_script.Should().Be(Context.Syntax.CurrentDatabase);
        });
    }

    [Fact]
    public async Task Can_handle_large_scripts()
    {
        var db = TestConfig.RandomDatabase();

        var parent = TestConfig.CreateRandomTempDirectory();
        var knownFolders = FoldersConfiguration.Default(null);
        GrateMigrator? migrator;

        var folder = new DirectoryInfo(Path.Combine(parent.ToString(), knownFolders[Up]!.Path));

        const string filename = "large_file.sql";

        CreateLargeDummySql(folder, filename: filename);
        await using (migrator = Context.GetMigrator(db, parent, knownFolders))
        {
            await migrator.Migrate();
        }

        string fileContent = await File.ReadAllTextAsync(Path.Combine(folder.ToString(), filename));

        string[] scripts;
        string sql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRun")}";

        await using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(sql)).ToArray();
        }

        scripts.First().Should().Be(fileContent);
    }

}
