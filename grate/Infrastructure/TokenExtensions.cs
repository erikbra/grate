using grate.Configuration;

namespace grate.Infrastructure;

public static class TokenExtensions
{
    //helpers to centralise converting various types to a token

    public static string? ToToken(this MigrationsFolder? folder)
    {
        return folder?.Path.Name;
    }

}