namespace grate.Configuration;

public static class ApplicationInfo
{
    public static string Name => "grate";
    public static string Version => typeof(ApplicationInfo).Assembly.GetName().Version?.ToString() ?? "0.0.0.1";
}
