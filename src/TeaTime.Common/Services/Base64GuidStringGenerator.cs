namespace TeaTime.Common.Services;

using System;
using System.Threading.Tasks;
using Abstractions;

/// <summary>
/// Generates string using Guids that are base64 encoded
/// </summary>
public class Base64GuidStringGenerator : IIdGenerator<string>
{
    // Converting a 16 byte guid to a base64 string will result in a 22 byte string
    private const int StringSize = 22;

    public Task<string> GenerateAsync()
    {
        Span<byte> bytes = stackalloc byte[16];
        Guid.NewGuid().TryWriteBytes(bytes);
        var value = Base64UrlEncode(bytes);
        return Task.FromResult(value);
    }

    private static string Base64UrlEncode(ReadOnlySpan<byte> input)
    {
        // Even though our output string will only be 22 bytes,
        // we need to allocate 24 bytes because base64 decodes in multiples of 4
        Span<char> buffer = stackalloc char[24];
        Base64UrlEncode(input, buffer);
        return new string(buffer[..StringSize]);
    }

    private static void Base64UrlEncode(ReadOnlySpan<byte> input, Span<char> output)
    {
        // Use base64url encoding with no padding characters. See RFC 4648, Sec. 5.

        Convert.TryToBase64Chars(input, output, out _);

        // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
        for (var i = 0; i < StringSize; i++)
        {
            var ch = output[i];
            if (ch == '+')
            {
                output[i] = '-';
            }
            else if (ch == '/')
            {
                output[i] = '_';
            }
        }
    }
}
