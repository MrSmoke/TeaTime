namespace TeaTime.Slack.Services;

using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class SlackVerifyRequestMiddleware(RequestDelegate next,
    ISlackRequestVerifier requestVerifier,
    ILogger<SlackVerifyRequestMiddleware> logger)
{
    // Max size of a request body
    private const int MaxLength = 512 * 1024; // 512Kb

    public async Task InvokeAsync(HttpContext context)
    {
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

        var cancellationToken = context.RequestAborted;

        // Copy body to a memory stream so we can re-read
        var memoryStream = new MemoryStream();
        var bodyStream = context.Request.Body;
        await CopyToAsync(bodyStream, memoryStream, 1024, cancellationToken);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // Assign back to Body for the model binding
        context.Request.Body = memoryStream;
        context.Response.RegisterForDispose(memoryStream);

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

    private static async Task CopyToAsync(Stream source, Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(new Memory<byte>(buffer), cancellationToken)) != 0)
            {
                if (bytesRead > MaxLength)
                    throw new Exception("Body too big");

                await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
