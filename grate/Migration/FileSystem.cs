using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.IO.Path;
using static System.IO.SearchOption;
using static System.StringComparer;

namespace grate.Migration;

public static class FileSystem
{
    public static IEnumerable<FileSystemInfo> GetFiles(DirectoryInfo folderPath, string pattern, bool ignoreDirectoryNames = false)
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
}
