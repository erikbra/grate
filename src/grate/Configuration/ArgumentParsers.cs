using System.CommandLine.Parsing;

namespace grate.Configuration;

internal static class ArgumentParsers
{
    private static readonly char[] Delimiters = [',', ';'];

    // System.CommandLine beta3 has broken support for basic public constructors in order to provide better support for trimming assemblies.
    // Specify a parser to work around this, see https://github.com/dotnet/command-line-api/issues/1664
    public static CommandLineGrateEnvironment? ParseEnvironment(ArgumentResult result)
    {
        return result.Tokens.Any() 
            ? new CommandLineGrateEnvironment(result.Tokens.SelectMany(GetEnvironments)) 
            : null;
    }

    private static IEnumerable<string> GetEnvironments(Token token)
    {
        return token.Value.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
    }
}
