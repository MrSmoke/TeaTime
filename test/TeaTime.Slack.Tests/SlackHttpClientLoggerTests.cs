namespace TeaTime.Slack.Tests;

using System;
using Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class SlackHttpClientLoggerTests
{
    [Theory]
    [InlineData("https://hooks.slack.com/commands/T1/123/secret", "https://hooks.slack.com/commands/***")]
    [InlineData("https://hooks.slack.com/actions/T1/123/secret", "https://hooks.slack.com/actions/***")]
    [InlineData("https://hooks.slack-gov.com/actions/T1/123/secret", "https://hooks.slack-gov.com/actions/***")]
    [InlineData("https://slack.com/api/oauth.v2.access", "https://slack.com/api/oauth.v2.access")]
    [InlineData("https://slack.com/api/oauth.v2.access?code=abc", "https://slack.com/api/oauth.v2.access")]
    [InlineData("https://example.com/", "https://example.com/***")]
    public void Redact_HidesWebhookSecrets(string url, string expected)
    {
        Assert.Equal(expected, SlackHttpClientLogger.Redact(new Uri(url)));
    }

    [Fact]
    public void Registration_ResolvesClient()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSingleton<SlackHttpClientLogger>();
        services.AddHttpClient<ISlackApiClient, SlackApiClient>()
            .RemoveAllLoggers()
            .AddLogger<SlackHttpClientLogger>();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<ISlackApiClient>());
    }
}
