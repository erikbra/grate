namespace CommandLine.Common;

[AttributeUsage(AttributeTargets.Assembly)]
public class GrateExecutablePathAttribute(string path) : Attribute
{
    public string Path { get; set; } = path;
}

