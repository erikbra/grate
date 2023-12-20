using System;
using System.Security.Cryptography;
using System.Text;

namespace grate.Infrastructure;

public class HashGenerator : IHashGenerator
{
    private const string WindowsLineEnding = "\r\n";
    private const string UnixLineEnding = "\n";
    private const string MacLineEnding = "\r";

    public string Hash(string text)
    {
        var input = text.Replace(@"'", @"''");

        input = input
            .Replace(WindowsLineEnding, UnixLineEnding)
            .Replace(MacLineEnding, UnixLineEnding);

        var messageBytes = Encoding.UTF8.GetBytes(input);
        var hash = ComputeHash(messageBytes);

        return Convert.ToBase64String(hash);
    }

    private static byte[] ComputeHash(byte[] messageBytes)
    {
#pragma warning disable CA5351
        // The code that's violating the rule is on this line.
        using var alg = MD5.Create(); // consider to use SHA256
#pragma warning restore CA5351
        alg.ComputeHash(messageBytes);
        return alg.Hash!;
    }
}
