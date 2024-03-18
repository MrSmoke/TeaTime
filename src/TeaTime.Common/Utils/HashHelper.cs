namespace TeaTime.Common.Utils;

using System.Security.Cryptography;
using System.Text;

public static class HashHelper
{
    public static string HashStringSha256(byte[] key, string value)
    {
        var valueBytes = Encoding.UTF8.GetBytes(value);
        var hashBytes = HMACSHA256.HashData(key, valueBytes);
        return ToHex(hashBytes);
    }

    public static string ToHex(byte[] bytes)
    {
        var sBuilder = new StringBuilder();

        for (var i = 0; i < bytes.Length; i++)
            sBuilder.Append(bytes[i].ToString("x2"));

        return sBuilder.ToString();
    }
}
