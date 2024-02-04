using grate.Configuration;

namespace grate.Infrastructure;

internal static class TokenExtensions
{
    //helpers to centralise converting various types to a token

    public static string? ToToken(this MigrationsFolder? folder) => folder?.Path;
}
