using System.Text.RegularExpressions;
using grate.Configuration;

namespace SampleService.Extension;
public static class ConnectionStringExtension
{
    public static void SwitchDatabase(this GrateConfiguration grateConfiguration, string targetDatabase)
    {
        var pattern = new Regex("(.*;\\s*(?:Initial Catalog|Database)=)([^;]*)(.*)");
        var replacement = $"$1{targetDatabase}$3";
        var replaced = pattern.Replace(grateConfiguration.ConnectionString!, replacement);
        grateConfiguration.ConnectionString = replaced;
    }
}
