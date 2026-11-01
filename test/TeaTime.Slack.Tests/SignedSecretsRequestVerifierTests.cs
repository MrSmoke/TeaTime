namespace TeaTime.Slack.Tests;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Services;
using Xunit;

public class SignedSecretsRequestVerifierTests
{
    private const string Secret = "8f742231b10e8888abcd99yyyzzz85a5";
    private const string Body = "token=abc&team_id=T1&command=%2Ftea&text=run";
    private static readonly DateTimeOffset Now = DateTimeOffset.FromUnixTimeSeconds(1_700_000_000);

    [Fact]
    public async Task ValidSignature_Passes()
    {
        var ts = Now.ToUnixTimeSeconds();

        Assert.True(await VerifyAsync(Sign(ts, Body), ts, Body));
    }

    [Fact]
    public async Task TamperedBody_Fails()
    {
        var ts = Now.ToUnixTimeSeconds();

        Assert.False(await VerifyAsync(Sign(ts, Body), ts, Body + "x"));
    }

    [Fact]
    public async Task WrongLengthSignature_Fails()
    {
        var ts = Now.ToUnixTimeSeconds();

        Assert.False(await VerifyAsync("v0=abc", ts, Body));
    }

    [Theory]
    [InlineData(-301)]
    [InlineData(301)]
    public async Task TimestampOutsideSkew_Fails(int offsetSeconds)
    {
        var ts = Now.ToUnixTimeSeconds() + offsetSeconds;

        Assert.False(await VerifyAsync(Sign(ts, Body), ts, Body));
    }

    [Theory]
    [InlineData(-299)]
    [InlineData(299)]
    public async Task TimestampWithinSkew_Passes(int offsetSeconds)
    {
        var ts = Now.ToUnixTimeSeconds() + offsetSeconds;

        Assert.True(await VerifyAsync(Sign(ts, Body), ts, Body));
    }

    [Fact]
    public async Task ExtremeTimestamp_FailsWithoutThrowing()
    {
        // now - timestamp wraps to long.MinValue, which Math.Abs can't negate
        var ts = Now.ToUnixTimeSeconds() + long.MinValue;

        Assert.False(await VerifyAsync(Sign(ts, Body), ts, Body));
    }

    private static string Sign(long timestamp, string body)
    {
        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(Secret), Encoding.UTF8.GetBytes($"v0:{timestamp}:{body}"));
        return "v0=" + Convert.ToHexStringLower(hash);
    }

    private static Task<bool> VerifyAsync(string signature, long timestamp, string body)
    {
        var options = A.Fake<IOptionsMonitor<SignedSecretsRequestVerifierOptions>>();
        A.CallTo(() => options.CurrentValue).Returns(new SignedSecretsRequestVerifierOptions { SigningSecret = Secret });

        var verifier = new SignedSecretsRequestVerifier(new FixedTimeProvider(Now), options,
            NullLogger<SignedSecretsRequestVerifier>.Instance);

        var context = new DefaultHttpContext();
        context.Request.Headers["X-Slack-Signature"] = signature;
        context.Request.Headers["X-Slack-Request-Timestamp"] = timestamp.ToString();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

        return verifier.VerifyAsync(context.Request);
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
