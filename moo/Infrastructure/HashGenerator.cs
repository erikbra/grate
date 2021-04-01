using System;
using System.Security.Cryptography;
using System.Text;

namespace moo.Infrastructure
{
    public class HashGenerator : IHashGenerator
    {
        private const string WindowsLineEnding = "\r\n";
        private const string UnixLineEnding = "\n";
        private const string MacLineEnding = "\r";
        
        public string Hash(string text)
        {
            var input = text.Replace(@"'", @"''");
            input = input.Replace(WindowsLineEnding, UnixLineEnding).Replace(MacLineEnding, UnixLineEnding);
            
            var messageBytes =  Encoding.UTF8.GetBytes(input);
            var hash = ComputeHash(messageBytes);
            
            return Convert.ToBase64String(hash);
        }

        private static byte[] ComputeHash(byte[] messageBytes)
        {
            using var alg = new MD5CryptoServiceProvider();
            alg.ComputeHash(messageBytes);
            return alg.Hash!;
        }
    }
}