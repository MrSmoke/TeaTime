namespace TeaTime;

using Common.Abstractions;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

public class UrlGenerator(LinkGenerator linkGenerator,
    IHttpContextAccessor httpContextAccessor,
    IOptionsMonitor<UrlGeneratorOptions> options) : IUrlGenerator
{
    public string CreateAbsoluteUrl(string path)
    {
        var (scheme, host) = GetHost();

        return UriHelper.BuildAbsolute(
            scheme: scheme,
            host: host,
            path: path
        );
    }

    public string CreateAbsoluteUrlByName(string name)
    {
        var path = linkGenerator.GetPathByName(name);

        if (path is null)
            throw new TeaTimeException($"Unknown endpoint name '{name}'");

        return CreateAbsoluteUrl(path);
    }

    private (string Scheme, HostString Host) GetHost()
    {
        // Get self from config if it exists
        var selfHost = options.CurrentValue.SelfHost;
        if (selfHost is not null)
        {
            // Check if it's the default port and if so don't include it
            var host = selfHost.IsDefaultPort
                ? new HostString(selfHost.Host)
                : new HostString(selfHost.Host, selfHost.Port);

            return (selfHost.Scheme, host);
        }

        // Fallback to the current request
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is not null)
            return (httpContext.Request.Scheme, httpContext.Request.Host);

        throw new TeaTimeException("Could not determine host");
    }
}
