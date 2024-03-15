using static System.IO.Path;
using static System.IO.SearchOption;
using static System.StringComparer;

namespace grate.Infrastructure.FileSystem;

internal class PhysicalFileSystem: IFileSystem
{
    public IEnumerable<IFileSystemInfo> GetFiles(IDirectoryInfo folderPath, string pattern, bool ignoreDirectoryNames = false)
    {
        return ignoreDirectoryNames
            ? folderPath
                .EnumerateFileSystemInfos(pattern, AllDirectories).ToList()
                .OrderBy(f => GetFileNameWithoutExtension(f.FullName), CurrentCultureIgnoreCase)
            : folderPath
                .EnumerateFileSystemInfos(pattern, AllDirectories).ToList()
                .OrderBy(f =>
                    Combine(
                        GetRelativePath(folderPath.ToString(), GetDirectoryName(f.FullName)!),
                        GetFileNameWithoutExtension(f.FullName)),
                    CurrentCultureIgnoreCase);
    }
    
    public IDirectoryInfo Wrap(IDirectoryInfo root, string? subFolder) 
        => new PhysicalDirectoryInfo(new DirectoryInfo(Combine(root.ToString(), subFolder ?? "")));

    public void WriteAllText(string fileName, string? sql) => File.WriteAllText(fileName, sql);
    
}
