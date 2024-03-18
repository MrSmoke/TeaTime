namespace TeaTime.Slack.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Client;
using Configuration;
using Models.Requests;
using Models.Responses;

internal class SlackAuthenticationService(ISlackApiClient slackApiClient, IOptionsMonitor<SlackOptions> options)
    : ISlackAuthenticationService
{
    private static readonly string[] OAuthScopes =
    [
        "commands",
        "incoming-webhook"
    ];

    public bool OAuthEnabled()
    {
        var oauth = options.CurrentValue.OAuth;
        return oauth is not null && oauth.IsValid();
    }

    public string BuildAuthorizeUrl()
    {
        var oauth = GetOptions();

        const string baseUrl = "https://slack.com/oauth/v2/authorize";

        return QueryHelpers.AddQueryString(baseUrl, new KeyValuePair<string, StringValues>[]
        {
            new("client_id", oauth.ClientId),
            new("scope", OAuthScopes)
        });
    }

    public Task<OAuthTokenResponse> GetOAuthTokenAsync(string code, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        var oauth = GetOptions();

        return slackApiClient.GetOAuthTokenAsync(new OAuthTokenRequest
        {
            ClientId = oauth.ClientId,
            ClientSecret = oauth.ClientSecret,
            RedirectUri = oauth.RedirectUri,
            Code = code
        });
    }

    private SlackOAuthOptions GetOptions()
    {
        var oauth = options.CurrentValue.OAuth;
        if (oauth is null || !oauth.IsValid())
            throw new InvalidOperationException("Oauth is not configured");

        return oauth;
    }
}
