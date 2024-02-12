using System.Text.RegularExpressions;
using grate.Configuration;
using grate.Migration;

namespace grate.Infrastructure;
internal static class ConnectionStringExtension
{
    public static string? GetAdminConnectionString(this IDatabase database, GrateConfiguration grateConfiguration)
    {
        if (grateConfiguration.AdminConnectionString is not null)
        {
            return grateConfiguration.AdminConnectionString;
        }
        if (grateConfiguration.ConnectionString is not null)
        {
            return WithAdminDb(grateConfiguration.ConnectionString, database.MasterDatabaseName);
        }
        return default;

    }
    private static string WithAdminDb(string connectionString, string masterDatabaseName)
    {
        var pattern = new Regex("(.*;\\s*(?:Initial Catalog|Database)=)([^;]*)(.*)");
        var replacement = $"$1{masterDatabaseName}$3";
        var replaced = pattern.Replace(connectionString, replacement);
        return replaced;
    }
}
