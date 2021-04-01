namespace moo.Infrastructure
{
    public static class StringExtensions
    {
        public static string ReplaceToken(this string s, string token, string value) => s.Replace($"{{token}}", value);
        
    }
}