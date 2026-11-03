namespace TeaTime.Slack.Tests;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Client;
using Microsoft.Extensions.Logging;
using Models.SlashCommands;
using Xunit;

public class SlackApiClientTests
{
    private const string ResponseUrl = "https://hooks.slack.com/commands/T1/1/secret";

    [Fact]
    public async Task PostResponse_Rejected_LogsWarningWithSlackErrorButNotUrl()
    {
        var logger = new ListLogger<SlackApiClient>();
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("expired_url")
        }, logger);

        await client.PostResponseAsync(ResponseUrl, new SlashCommandResponse("hi", ResponseType.User),
            TestContext.Current.CancellationToken);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Contains("404", entry.Message);
        Assert.Contains("expired_url", entry.Message);
        Assert.DoesNotContain("secret", entry.Message);
    }

    [Fact]
    public async Task PostResponse_NetworkFailure_LogsErrorWithoutThrowing()
    {
        var logger = new ListLogger<SlackApiClient>();
        var client = CreateClient(_ => throw new HttpRequestException("connection refused"), logger);

        await client.PostResponseAsync(ResponseUrl, new SlashCommandResponse("hi", ResponseType.User),
            TestContext.Current.CancellationToken);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, entry.Level);
        Assert.IsType<HttpRequestException>(entry.Exception);
    }

    [Fact]
    public async Task PostResponse_Success_LogsNothing()
    {
        var logger = new ListLogger<SlackApiClient>();
        var client = CreateClient(_ => new HttpResponseMessage(HttpStatusCode.OK), logger);

        await client.PostResponseAsync(ResponseUrl, new SlashCommandResponse("hi", ResponseType.User),
            TestContext.Current.CancellationToken);

        Assert.Empty(logger.Entries);
    }

    private static SlackApiClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> respond,
        ILogger<SlackApiClient> logger) => new(new HttpClient(new StubHandler(respond)), logger);

    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) => Task.FromResult(respond(request));
    }

    private sealed record LogEntry(LogLevel Level, string Message, Exception? Exception);

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
    }
}
