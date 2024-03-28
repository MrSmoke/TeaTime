namespace TeaTime.Data.MySql.Tests;

using System;

public static class TestData
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static long NewInt64() => Random.Shared.NextInt64();

    public static string NewString(int length = 30)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
            chars[i] = Chars[Random.Shared.Next(Chars.Length)];

        return new string(chars);
    }

    // MySql DateTime only stores with a precision of second
    public static DateTimeOffset MySqlNow() =>
        DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
}
