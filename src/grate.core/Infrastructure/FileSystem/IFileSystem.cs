namespace grate.Infrastructure.FileSystem;

public interface IFileSystem
{
    IEnumerable<IFileSystemInfo> GetFiles(IDirectoryInfo folderPath, string pattern, bool ignoreDirectoryNames = false);
    IDirectoryInfo Wrap(IDirectoryInfo root, string? subFolder);
    
    void WriteAllText(string fileName, string? sql);
    
    public void WriteContent(IDirectoryInfo root, string path, string filename, string? sql) =>
        WriteContent(Wrap(root, path), filename, sql);
    
    void WriteContent(IDirectoryInfo? path, string filename, string? content)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (!path.Exists)
        {
            path.Create();
        }

        WriteAllText(Path.Combine(path.ToString(), filename), content);
    }
}
