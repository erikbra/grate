using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace grate.Migration;

public static class FileSystem
{
    public static IEnumerable<FileSystemInfo> GetFiles(DirectoryInfo folderPath, string pattern)
    {
        var fileList = folderPath
            .EnumerateFileSystemInfos(pattern, SearchOption.AllDirectories).ToList()
            .OrderBy(f => Path.GetRelativePath(folderPath.ToString(), f.FullName), StringComparer.CurrentCultureIgnoreCase);

        return fileList;
    }
}
