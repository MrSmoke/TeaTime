namespace TeaTime.Slack.Client;

using System;
using System.Net.Http;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Logging;

/// <summary>
/// Replaces the default HttpClient loggers: one line per request, with webhook URLs redacted
/// </summary>
internal sealed class SlackHttpClientLogger(ILogger<SlackHttpClientLogger> logger) : IHttpClientLogger
{
    private static readonly string[] ApiHosts = ["slack.com", "slack-gov.com"];

    public object? LogRequestStart(HttpRequestMessage request) => null;

    public void LogRequestStop(object? context, HttpRequestMessage request, HttpResponseMessage response,
        TimeSpan elapsed)
    {
        logger.LogInformation("Slack {Method} {Uri} responded {StatusCode} in {ElapsedMs:0}ms",
            request.Method, Redact(request.RequestUri), (int)response.StatusCode, elapsed.TotalMilliseconds);
    }

    public void LogRequestFailed(object? context, HttpRequestMessage request, HttpResponseMessage? response,
        Exception exception, TimeSpan elapsed)
    {
        // Callers log the failure with context, so only note it here without the stack trace
        logger.LogInformation("Slack {Method} {Uri} failed after {ElapsedMs:0}ms: {Error}",
            request.Method, Redact(request.RequestUri), elapsed.TotalMilliseconds, exception.GetType().Name);
    }

    internal static string Redact(Uri? uri)
    {
        if (uri is null)
            return "(none)";

        // Web API URLs carry no secrets; drop any query string anyway
        if (Array.IndexOf(ApiHosts, uri.Host) >= 0)
            return uri.GetLeftPart(UriPartial.Path);

        // Anything else (e.g. response_url on hooks.slack.com) is a credential; keep only the first segment
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var prefix = segments.Length > 0 ? "/" + segments[0] : "";

        return $"{uri.Scheme}://{uri.Host}{prefix}/***";
    }
}
