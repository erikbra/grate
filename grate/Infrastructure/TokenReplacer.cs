using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace grate.Infrastructure
{
    public static class TokenReplacer // When we have a second impl we'll add an interface :)
    {
        public static string ReplaceTokens(Dictionary<string, string?> tokens, string? textToReplace)
        {
            if (string.IsNullOrEmpty(textToReplace))
            {
                return string.Empty;
            }


            //This regex is magic to me, but it worked in Roundhouse!
            var regex = new Regex("{{(?<key>\\w+)}}");

            string output = regex.Replace(textToReplace, m =>
            {
                string key = "";

                key = m.Groups["key"].Value;
                if (!tokens.ContainsKey(key))
                {
                    return "{{" + key + "}}"; //leave unrecognised token alone
                }

                var value = tokens[key];
                return value ?? string.Empty;
            });

            return output;

        }

    }
}
