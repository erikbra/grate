using System.IO;
using static System.StringComparison;

namespace moo.Infrastructure
{
    public class MooEnvironment
    {
        public string Current { get; }
        private const string EnvironmentMarker = ".ENV.";

        public MooEnvironment(string current) => Current = current;

        public bool ShouldRun(string path) => !IsEnvironmentFile(path) || IsForCurrentEnvironment(path);

        private bool IsForCurrentEnvironment(string path) =>
            FileName(path).StartsWith($"{Current}.", InvariantCultureIgnoreCase) ||
            FileName(path).Contains($".{Current}.", InvariantCultureIgnoreCase);
        
        private static bool IsEnvironmentFile(string fileName) => fileName.Contains(EnvironmentMarker, InvariantCultureIgnoreCase);
        private static string FileName(string path) => new FileInfo(path).Name;
    }
}