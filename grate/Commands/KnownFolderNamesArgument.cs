using System;
using System.Linq;
using grate.Configuration;

namespace grate.Commands;

public static class KnownFolderNamesArgument
{
    public static IKnownFolderNames Parse(string? commandLine)
    {
        var def = KnownFolderNames.Default;
        var replacements = (commandLine?.Split(",", StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>());

        foreach (var replacement in replacements)
        {
            var tokens = replacement.Split(':', 2);
            var key = tokens.First();
            var val = tokens.Last();

            var prop = def.GetType().GetProperties()
                .First(p => p.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            
            prop.SetValue(def, val);
        }

        return def;
    }
}
