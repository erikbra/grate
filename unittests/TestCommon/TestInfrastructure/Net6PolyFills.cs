namespace TestCommon.TestInfrastructure;
using System.IO;

#if NET6_0
public static class Net6PolyFills
{
    public static class Directory
    {
        public static DirectoryInfo CreateTempSubdirectory() 
            => CreateTempSubdirectory(Path.GetRandomFileName());

        public static DirectoryInfo CreateTempSubdirectory(string name)
        {
            return new DirectoryInfo(Path.GetTempPath()).CreateSubdirectory(name);
        }
    }
}
#endif
