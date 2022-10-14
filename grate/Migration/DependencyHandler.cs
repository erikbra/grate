using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using grate.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace grate.Migration;

public static class DependencyHandler
{

    public static IEnumerable<FileSystemInfo> HandleDependencies(IEnumerable<FileSystemInfo> fileList, DirectoryInfo folderPath, string dependencyPattern, string splitter, ILogger<GrateMigrator> logger)
    {

        logger.Log(LogLevel.Debug, "Checking each file for dependencies");
        var re = new Regex(dependencyPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline, TimeSpan.FromSeconds(1));
        var reSplitter = new Regex(splitter, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline, TimeSpan.FromSeconds(1));
        var reWildcard = new Regex("[*?#]+", RegexOptions.Compiled);
        var ts = new TopologicalSorter(fileList.Count(), logger);
        var savedFilenames = fileList.Select(n => n.FullName.Remove(0, folderPath.FullName.Length + 1)).ToList();

        foreach (var file in fileList)
        {
            var fileContents = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(File.ReadAllText(file.FullName))).Replace("\r", "");
            var fileName = file.FullName.Remove(0, folderPath.FullName.Length + 1);

            var dependencies = new List<string>();
            var matches = re.Matches(fileContents);
            if (matches.Count == 0)
            {
                ts.AddToList(fileName, dependencies.ToArray());
                continue;
            }

            foreach (Match m in matches)
            {
                dependencies.AddRange(reSplitter.Replace(m.Groups[2].Value, "\r").Replace('\\', Path.DirectorySeparatorChar).Split("\r").Where(d => !string.IsNullOrEmpty(d)).ToArray());
            }

            dependencies = WildcardHandler(dependencies);
            ts.AddToList(fileName, dependencies.ToArray());

            logger.Log(LogLevel.Debug, "{File} has the following declared dependencies: {Join}", file, string.Join("-->", dependencies));

        }

        var so = ts.Sort();
        var fl = fileList.ToArray();
        var orderedFileList = new List<FileSystemInfo>();
        for (var i = so.Length - 1; i >= 0; i--)
            orderedFileList.Add(fl[so[i]]);

        foreach(var file in orderedFileList)
            Console.WriteLine(file.FullName);

        return orderedFileList;

        List<string> WildcardHandler(List<string> dependencies)
        {

            var ret = new List<string>();

            foreach (var dependency in dependencies)
            {
                if (reWildcard.IsMatch(dependency))
                {
                    var dependencyWithRegEx = dependency.Replace("*", "\xff").Replace("?", "\xfe").Replace("#", "\xfd");
                    dependencyWithRegEx = Regex.Escape(dependencyWithRegEx);
                    dependencyWithRegEx = dependencyWithRegEx.Replace("\xfd", "[0-9]").Replace("\xfe", ".").Replace("\xff", ".*[^.]");

                    var regex = new Regex(dependencyWithRegEx, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    ret.AddRange(savedFilenames.Where(entry => regex.IsMatch(entry)));
                }
                else
                    ret.Add(dependency);
            }
            return new HashSet<string>(ret).ToList();
        }
    }
}
