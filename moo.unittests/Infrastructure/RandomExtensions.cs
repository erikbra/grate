using System;

namespace moo.unittests.Infrastructure
{
    public static class RandomExtensions
    {
        private static char[] DefaultAllowedChars = "ABCDEFHIJKLMNOPQRSTUVWXYZabcdefhijklmnopqrstuvwxyz".ToCharArray();

        public static string GetString(this Random random, int length, string allowedChars)
            => GetString(random, length, allowedChars.ToCharArray());
        
        public static string GetString(this Random random, int length, char[] allowedChars = null)
        {
            allowedChars ??= DefaultAllowedChars;
            
            var bytes = new byte[length];
            random.NextBytes(bytes);

            var allowedCharLength = (byte) allowedChars.Length;

            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[bytes[i] % allowedCharLength];
            }

            return new string(chars);
        }
        
    }
}