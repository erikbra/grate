namespace grate.Infrastructure.FileSystem;

public record PhysicalDirectoryInfo(DirectoryInfo DirectoryInfo): IDirectoryInfo
{
    public PhysicalDirectoryInfo(string directory) : this(new DirectoryInfo(directory)) { }

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string pattern, SearchOption allDirectories) 
        => DirectoryInfo.EnumerateFileSystemInfos(pattern, allDirectories).Select(Get);

    public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
        => DirectoryInfo.EnumerateFileSystemInfos().Select(Get);

    public FileInfo[] GetFiles(string sql, SearchOption allDirectories) 
        => DirectoryInfo.GetFiles(sql, allDirectories);

    public IDirectoryInfo? Parent => DirectoryInfo.Parent is null 
                            ? null 
                            : new PhysicalDirectoryInfo(DirectoryInfo.Parent);

    private static PhysicalFileSystemInfo Get(FileSystemInfo x) => new(x);

    public bool Exists => DirectoryInfo.Exists;
    public void Create() => DirectoryInfo.Create();

    public string FullName => DirectoryInfo.FullName;
    public override string ToString() => DirectoryInfo.ToString();
    
    public static implicit operator PhysicalDirectoryInfo(DirectoryInfo directory) => new(directory);
    public static implicit operator PhysicalDirectoryInfo(string directory) => new(directory);
    
}
