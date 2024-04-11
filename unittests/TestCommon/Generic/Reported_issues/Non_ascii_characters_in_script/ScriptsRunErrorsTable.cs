using System.Security.Claims;
using Dapper;
using FluentAssertions;
using grate.Configuration;
using grate.Exceptions;
using TestCommon.Generic.Running_MigrationScripts;
using TestCommon.TestInfrastructure;
using Xunit.Abstractions;

namespace TestCommon.Generic.Reported_issues.Non_ascii_characters_in_script;

public abstract class ScriptsRunErrorsTable : MigrationsScriptsBase
{
    private readonly string _sql;

    protected ScriptsRunErrorsTable(IGrateTestContext context, ITestOutputHelper testOutput) : base(context, testOutput)
    {
        _sql = $"""
                Klønete bråkjekkas
                {Context.Sql.LineComment} This is a comment: комментарий
                {Context.Sql.SelectVersion}
                """;
    }


    [Theory]
    [InlineData("Blåbærkake")]
    [InlineData("لا أحب الطقس الممطر")]
    [InlineData("Я коричневая черепаха")]
    [InlineData("✨")]
    [InlineData("🎉")]
    [InlineData("👍")]
    public async Task Repository_path(string repositoryPath)
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        var filename = "1_script_with_utf_characters.sql";
        WriteSql(Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path), filename, _sql);
        
       var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .WithRepositoryPath(repositoryPath)
            .Build();
        
       await using (var migrator = Context.Migrator.WithConfiguration(config))
       {
           await Assert.ThrowsAnyAsync<MigrationFailed>(() => migrator.Migrate());
       }

       string[] repositoryPaths;
       string selectSql = $"SELECT repository_path FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

       using (var conn = Context.CreateDbConnection(db))
       {
           repositoryPaths = (await conn.QueryAsync<string>(selectSql)).ToArray();
       }

       repositoryPaths.Should().HaveCount(1);
       repositoryPaths.First().Should().Be(repositoryPath);
        
       //await Context.DropDatabase(db);
    }
    
    [Theory]
    [InlineData("Værsjooon ein")]
    [InlineData("الأول")]
    [InlineData("первый")]
    public async Task Version(string version)
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        var filename = "1_a_script.sql";
        WriteSql(Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path), filename, _sql);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .WithVersion(version)
            .Build();
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await Assert.ThrowsAnyAsync<MigrationFailed>(() => migrator.Migrate());
        }

        string[] versions;
        string selectSql = $"SELECT version FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (var conn = Context.CreateDbConnection(db))
        {
            versions = (await conn.QueryAsync<string>(selectSql)).ToArray();
        }

        versions.Should().HaveCount(1);
        versions.First().Should().Be(version);
        
        //await Context.DropDatabase(db);
    }
    
    
    [Theory]
    [InlineData("det_første.sql")]
    [InlineData("الأول.sql")]
    [InlineData("первый_script.sql")]
    [InlineData("✨.sql")]
    [InlineData("🎉.sql")]
    [InlineData("👍.sql")]
    public async Task Script_name(string scriptName)
    {
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        var filename = scriptName;
        WriteSql(Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path), filename, _sql);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await Assert.ThrowsAnyAsync<MigrationFailed>(() => migrator.Migrate());
        }

        string[] scriptNames;
        string selectSql = $"SELECT script_name FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (var conn = Context.CreateDbConnection(db))
        {
            scriptNames = (await conn.QueryAsync<string>(selectSql)).ToArray();
        }

        scriptNames.Should().HaveCount(1);
        scriptNames.First().Should().Be(scriptName);
        
        //await Context.DropDatabase(db);
    }
    
    [Theory]
    [InlineData("Blåbærkake")]
    [InlineData("لا أحب الطقس الممطر")]
    [InlineData("Я коричневая черепаха")]
    [InlineData("✨")]
    [InlineData("🎉")]
    [InlineData("👍")]
    public async Task Text_of_script(string characters)
    {
        var sql = $"""
                   {characters}
                   {Context.Sql.LineComment} This is a comment: {characters}
                   {Context.Sql.SelectVersion}
                   """;
        
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        var filename = "1_script_with_utf_characters.sql";
        WriteSql(Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path), filename, sql);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await Assert.ThrowsAnyAsync<MigrationFailed>(() => migrator.Migrate());
        }

        string[] scripts;
        string selectSql = $"SELECT text_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (var conn = Context.CreateDbConnection(db))
        {
            scripts = (await conn.QueryAsync<string>(selectSql)).ToArray();
        }

        scripts.Should().HaveCount(1);
        scripts.First().Should().Be(sql);
        scripts.First().Should().Contain(characters);
        
        //await Context.DropDatabase(db);
    }
    
    [Theory]
    [InlineData("Blåbærkake")]
    [InlineData("لا أحب الطقس الممطر")]
    [InlineData("Я коричневая черепаха")]
    [InlineData("✨")]
    [InlineData("🎉")]
    [InlineData("👍")]
    public async Task Erroneous_part_of_script(string characters)
    {
        var sql = $"""
                   {characters}
                   {Context.Sql.SelectVersion}
                   """;
        
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        var filename = "1_script_with_utf_characters.sql";
        WriteSql(Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path), filename, sql);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await Assert.ThrowsAnyAsync<MigrationFailed>(() => migrator.Migrate());
        }

        string[] errors;
        string selectSql = $"SELECT erroneous_part_of_script FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (var conn = Context.CreateDbConnection(db))
        {
            errors = (await conn.QueryAsync<string>(selectSql)).ToArray();
        }

        errors.Should().HaveCount(1);
        errors.First().Should().Contain(characters);
        
        //await Context.DropDatabase(db);
    }
    
    
    [Theory]
    [InlineData("Blåbærkake")]
    [InlineData("لا أحب الطقس الممطر")]
    [InlineData("Я коричневая черепаха")]
    [InlineData("✨")]
    [InlineData("🎉")]
    [InlineData("👍")]
    public async Task Error_message(string characters)
    {
        var sql = $"""
                   {characters}!
                   {Context.Sql.SelectVersion}
                   """;
        
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        var filename = "1_script_with_utf_characters.sql";
        WriteSql(Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path), filename, sql);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await Assert.ThrowsAnyAsync<MigrationFailed>(() => migrator.Migrate());
        }

        string[] errors;
        string selectSql = $"SELECT error_message FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (var conn = Context.CreateDbConnection(db))
        {
            errors = (await conn.QueryAsync<string>(selectSql)).ToArray();
        }

        errors.Should().HaveCount(1);
        // This doesn't work well for MariaDb with 🎉 and 👍, they are only '?' in the actual error message from the DB 
        //errors.First().Should().Contain(characters);
        
        //await Context.DropDatabase(db);
    }
    
    
    [Theory]
    [InlineData("JanBanan")]
    [InlineData("Blåbærkake")]
    // [InlineData("لا أحب الطقس الممطر")]
    // [InlineData("Я коричневая черепаха")]
    // [InlineData("✨")]
    // [InlineData("🎉")]
    // [InlineData("👍")]
    public async Task Entered_by_does_not_bootstrap_well_with_unicode(string enteredBy)
    {
        var sql = $"""
                   {enteredBy}
                   {Context.Sql.SelectVersion}
                   """;
        
        var db = TestConfig.RandomDatabase();

        var parent = CreateRandomTempDirectory();
        var knownFolders = Folders.Default;
        var filename = "1_script_with_utf_characters.sql";
        WriteSql(Wrap(parent, knownFolders[KnownFolderKeys.Up]!.Path), filename, sql);
        
        var config = GrateConfigurationBuilder.Create(Context.DefaultConfiguration)
            .WithConnectionString(Context.ConnectionString(db))
            .WithFolders(knownFolders)
            .WithSqlFilesDirectory(parent)
            .Build();

        Thread.CurrentPrincipal =
            new ClaimsPrincipal(new[] { new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, enteredBy) }) }); 
        
        await using (var migrator = Context.Migrator.WithConfiguration(config))
        {
            await Assert.ThrowsAnyAsync<MigrationFailed>(() => migrator.Migrate());
        }

        string[] user;
        string selectSql = $"SELECT entered_by FROM {Context.Syntax.TableWithSchema("grate", "ScriptsRunErrors")}";

        using (var conn = Context.CreateDbConnection(db))
        {
            user = (await conn.QueryAsync<string>(selectSql)).ToArray();
        }

        user.Should().HaveCount(1);
        user.First().Should().Contain(enteredBy);
        
        //await Context.DropDatabase(db);
    }

}
