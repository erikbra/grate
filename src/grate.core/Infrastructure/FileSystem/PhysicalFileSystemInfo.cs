namespace grate.Infrastructure.FileSystem;

public record PhysicalFileSystemInfo(FileSystemInfo File): IFileSystemInfo
{
    public string FullName => File.FullName;
    public string Name => File.Name;
}
