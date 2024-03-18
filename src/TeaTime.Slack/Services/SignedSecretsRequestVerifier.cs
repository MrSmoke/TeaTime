namespace TeaTime.Slack.Services;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class SignedSecretsRequestVerifier(
    TimeProvider timeProvider,
    IOptionsMonitor<SignedSecretsRequestVerifierOptions> options,
    ILogger<SignedSecretsRequestVerifier> logger) : ISlackRequestVerifier
{
    private const string SignatureHeaderKey = "x-slack-signature";
    private const string TimestampHeaderKey = "x-slack-request-timestamp";
    private const string VersionNumber = "v0";

    public bool IsEnabled() => options.CurrentValue.Enabled;

    public async Task<bool> VerifyAsync(HttpRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!IsEnabled())
            return false;

        if (!TryGetSignature(request, out var slackSignature))
        {
            LogFailure("Missing signature");
            return false;
        }

        if (!request.Headers.TryGetValue(TimestampHeaderKey, out var timestampHeader))
        {
            LogFailure("Missing timestamp");
            return false;
        }

        if (!long.TryParse(timestampHeader, out var timestamp))
        {
            LogFailure("Failed to parse timestamp");
            return false;
        }

        // Read body and seek back to start
        var bodyStream = request.Body;

        string requestBody;
        using (var reader = new StreamReader(bodyStream, Encoding.UTF8, leaveOpen: true))
            requestBody = await reader.ReadToEndAsync(cancellationToken);

        bodyStream.Seek(0, SeekOrigin.Begin);

        return Verify(slackSignature, timestamp, requestBody);
    }

    private bool Verify(string signature, long timestamp, string requestBody)
    {
        ArgumentNullException.ThrowIfNull(signature);
        ArgumentNullException.ThrowIfNull(requestBody);

        var now = timeProvider.GetUtcNow().ToUnixTimeSeconds();

        var optionValue = options.CurrentValue;

        if (now - timestamp > optionValue.ClockSkew.TotalSeconds)
        {
            logger.LogWarning("Request verification failed. Too much time has passed since timestamp");
            return false;
        }

        // This shouldn't be null (options validation) but just in case
        if (optionValue.SigningSecret is null)
            return false;

        // todo: store the bytes and use OnChange to update
        var keyBytes = Encoding.UTF8.GetBytes(optionValue.SigningSecret);

        // Hash our data
        var signatureString = VersionNumber + ':' + timestamp + ':' + requestBody;
        var hashString = HashHelper.HashStringSha256(keyBytes, signatureString);
        var mySignature = VersionNumber + '=' + hashString;

        // Compare
        return mySignature.Equals(signature);
    }

    private static bool TryGetSignature(HttpRequest request, [NotNullWhen(true)] out string? signature)
    {
        if (!request.Headers.TryGetValue(SignatureHeaderKey, out var slackSignatureValues))
        {
            signature = null;
            return false;
        }

        string? slackSignature = slackSignatureValues;
        if (string.IsNullOrEmpty(slackSignature))
        {
            signature = null;
            return false;
        }

        signature = slackSignature;
        return true;
    }

    private void LogFailure(string reason) => logger.LogWarning("Request verification failed. {Reason}", reason);
}
