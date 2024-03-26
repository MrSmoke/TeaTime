namespace TeaTime.Common.Services;

using System;
using System.Buffers;
using System.Numerics;
using System.Threading.Tasks;
using Abstractions;

/// <summary>
/// Generates string using Guids that are base64 encoded
/// </summary>
public class GuidBasedStringGenerator : IIdGenerator<string>
{
    public ValueTask<string> GenerateAsync()
    {
        Span<byte> bytes = stackalloc byte[16];
        Guid.NewGuid().TryWriteBytes(bytes);
        return new ValueTask<string>(ToString(bytes));
    }

    private static string ToString(ReadOnlySpan<byte> toConvert)
    {
        const string alphabet = "0123456789abcdefghijklmnopqrstuvwxyz";

        // A length of 24 will truncate the last few bytes sometimes but that should be fine
        const int outBufferSize = 24;

        var dividend = new BigInteger(toConvert, isUnsigned: true);

        var outBuffer = ArrayPool<char>.Shared.Rent(outBufferSize);

        try
        {
            for (var i = 0; i < outBufferSize; i++)
            {
                dividend = BigInteger.DivRem(dividend, alphabet.Length, out var remainder);

                var nextChar = alphabet[Math.Abs((int)remainder)];
                outBuffer[i] = nextChar;
            }

            return new string(outBuffer[..outBufferSize]);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(outBuffer);
        }
    }
}
