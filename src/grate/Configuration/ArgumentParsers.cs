using System.CommandLine.Parsing;
using grate.Infrastructure;

namespace grate.Configuration
{
    public static class ArgumentParsers
    {
        // System.CommandLine beta3 has broken support for basic public constructors in order to provide better support for trimming assemblies.
        // Specify a parser to work around this, see https://github.com/dotnet/command-line-api/issues/1664

        public static GrateEnvironment? ParseEnvironment(ArgumentResult result)
        {
            if (result.Tokens.Count == 1)
            {
                return new GrateEnvironment(result.Tokens[0].Value);
            }

            result.ErrorMessage = $"Arg specified multiple times.";

            return default;
        }
    }
}
