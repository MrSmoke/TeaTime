namespace TeaTime.Slack.Services;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;

public class SlackVerifyRequestMiddleware(RequestDelegate next,
    ISlackRequestVerifier requestVerifier,
    ILogger<SlackVerifyRequestMiddleware> logger)
{
    // Max size of a request body
    // This is because we need to read the body to hash it but we don't want to fill the memory with malicious requests
    private const int MaxLength = 128 * 1024; //128 Kb for no real reason

    public async Task InvokeAsync(HttpContext context)
    {
        var cancellationToken = context.RequestAborted;

        // Check if the middleware needs to run for this request
        if (!IsForRequest(context))
        {
            await next(context);
            return;
        }

        // We only need to run this middleware if request verification is enabled
        if (!requestVerifier.IsEnabled())
        {
            logger.LogInformation("Request verification is not enabled. Skipping...");
            await next(context);
            return;
        }

        // Set the max body size for this request now that we know we need to read it for hashing
        SetMaxBodySize(context);

        // Copy body to a memory stream so we can re-read
        await ReplaceRequestBodyAsync(context, cancellationToken);

        // Verify request
        if (!await requestVerifier.VerifyAsync(context.Request, cancellationToken))
        {
            logger.LogInformation("Slack request verification failed");
            context.Response.StatusCode = 400;
            return;
        }

        logger.LogDebug("Slack request verification successful");

        await next(context);
    }

    private static bool IsForRequest(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        return endpoint?.Metadata.GetMetadata<SlackVerifyRequestAttribute>() != null;
    }

    private static void SetMaxBodySize(HttpContext context)
    {
        var maxRequestBodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
        if (maxRequestBodySizeFeature is null)
            throw new InvalidOperationException("IHttpMaxRequestBodySizeFeature is required");

        maxRequestBodySizeFeature.MaxRequestBodySize = MaxLength;
    }

    private static async Task ReplaceRequestBodyAsync(HttpContext context, CancellationToken cancellationToken)
    {
        // todo: replace with recyclable memory stream
        var memoryStream = new MemoryStream();
        context.Response.RegisterForDispose(memoryStream);

        await context.Request.Body.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Assign back to Body for the model binding
        context.Request.Body = memoryStream;
    }
}
