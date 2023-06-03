using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace grate.Migration;

public static class FileSystem
{
    public static IEnumerable<FileSystemInfo> GetFiles(DirectoryInfo folderPath, string pattern, bool ignoreDirectoryNames = false)
    {
        return ignoreDirectoryNames 
            ? folderPath
                .EnumerateFileSystemInfos(pattern, SearchOption.AllDirectories).ToList()
                .OrderBy(f => Path.GetFileNameWithoutExtension(f.FullName), StringComparer.CurrentCultureIgnoreCase) 
            : folderPath
                .EnumerateFileSystemInfos(pattern, SearchOption.AllDirectories).ToList()
                .OrderBy(f => Path.GetRelativePath(folderPath.ToString(), f.FullName), StringComparer.CurrentCultureIgnoreCase);
    }
}
