namespace grate.Infrastructure.FileSystem;

public interface IDirectoryInfo
{
    IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string pattern, SearchOption allDirectories);
    IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos();
    FileInfo[] GetFiles(string sql, SearchOption allDirectories);
    IDirectoryInfo? Parent { get; }
    string FullName { get; }
    bool Exists { get; }
    void Create();
    
    string ToString();
}
