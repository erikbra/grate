using System.IO;
using FluentAssertions;
using grate.Configuration;
using NUnit.Framework;

namespace grate.unittests.Basic.Configuration_;

// ReSharper disable once InconsistentNaming
public class Folder_
{
    [Test]
    public void Path_is_not_null_when_initialized_with_a_full_path()
    {
        var path = new DirectoryInfo("/tmp/full/path");
        var f = new Folder("a folder", path);
        f.Path.Should().Be(path);
    }
    
    [Test]
    public void Path_is_null_when_root_is_not_set()
    {
        var f = new Folder("a folder", "some/folder");
        f.Path.Should().BeNull();
    }
  
    [Test]
    public void Path_is_no_longer_null_after_root_is_set()
    {
        var f = new Folder("a folder", "some/folder");
        f.Path.Should().BeNull();
        f.SetRoot(new DirectoryInfo("/tmp/parent"));
        f.Path.Should().NotBeNull();
        f.Path?.ToString().Should().Be("/tmp/parent/some/folder");
    }
    
    [Test]
    public void IsRooted_is_true_when_initialized_with_a_full_path()
    {
        var path = new DirectoryInfo("/tmp/full/path");
        var f = new Folder("a folder", path);
        f.IsRooted().Should().BeTrue();
    }
    
    [Test]
    public void IsRooted_is_false_when_root_is_not_set()
    {
        var f = new Folder("a folder", "some/folder");
        f.IsRooted().Should().BeFalse();
    }
  
    [Test]
    public void IsRooted_is_no_longer_false_after_root_is_set()
    {
        var f = new Folder("a folder", "some/folder");
        f.IsRooted().Should().BeFalse();
        f.SetRoot(new DirectoryInfo("/tmp/parent"));
        f.IsRooted().Should().BeTrue();
    }
    
}
