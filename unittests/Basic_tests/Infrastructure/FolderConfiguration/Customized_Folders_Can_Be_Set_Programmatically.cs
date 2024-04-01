using System.Collections.Immutable;
using FluentAssertions;
using grate.Configuration;

namespace Basic_tests.Infrastructure.FolderConfiguration;

// ReSharper disable once InconsistentNaming
public class Customized_Folders_Can_Be_Set_Programmatically
{
    [Fact]
    public void From_MigrationsFolder_list()
    {
        var folders = Folders.Create(
            new MigrationsFolder("structure"),
            new MigrationsFolder("randomstuff"),
            new MigrationsFolder("procedures"),
            new MigrationsFolder("security")
            );
        var items = folders.Values.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Should().Be(folders["structure"]);
            items[1].Should().Be(folders["randomstuff"]);
            items[2].Should().Be(folders["procedures"]);
            items[3].Should().Be(folders["security"]);
        });
    }
    
    [Fact]
    public void From_Enumerable_of_MigrationsFolder()
    {
        var folders = Folders.Create(new List<MigrationsFolder>
            {
            new("structure"),
            new("randomstuff"),
            new("procedures"),
            new("security")}
        );
        var items = folders.Values.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Should().Be(folders["structure"]);
            items[1].Should().Be(folders["randomstuff"]);
            items[2].Should().Be(folders["procedures"]);
            items[3].Should().Be(folders["security"]);
        });
    }
    
    [Fact]
    public void From_Dictionary_of_MigrationsFolder()
    {
        var folders = Folders.Create(new Dictionary<string, MigrationsFolder>
            {
                {"structure", new("str") },
                {"randomstuff", new("rnd") },
                {"procedures", new("procs") },
                {"security", new("sec") }
            }
        );
        var items = folders.Values.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Should().Be(folders["structure"]);
            items[1].Should().Be(folders["randomstuff"]);
            items[2].Should().Be(folders["procedures"]);
            items[3].Should().Be(folders["security"]);
        });
    }
    
        
    [Fact]
    public void From_command_line_argument_style_single_string()
    {
        var folders = Folders.Create("nup=tables;sprocs=storedprocedures;views=projections");
        var items = folders.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Key.Should().Be("nup");
            items[0].Value!.Path.Should().Be("tables");
            
            items[1].Key.Should().Be("sprocs");
            items[1].Value!.Path.Should().Be("storedprocedures");
            
            items[2].Key.Should().Be("views");
            items[2].Value!.Path.Should().Be("projections");
        });
    }
    
    [Fact]
    public void From_multiple_string_arguments()
    {
        var folders = Folders.Create("zup=tables", "sprocs=path:storedprocedures,type:anytime", "views=projections");
        var items = folders.ToImmutableArray();

        Assert.Multiple(() =>
        {
            items[0].Key.Should().Be("zup");
            items[0].Value!.Path.Should().Be("tables");
            
            items[1].Key.Should().Be("sprocs");
            items[1].Value!.Path.Should().Be("storedprocedures");
            items[1].Value!.Type.Should().Be(MigrationType.AnyTime);
            
            items[2].Key.Should().Be("views");
            items[2].Value!.Path.Should().Be("projections");
        });
    }
    
    

}
