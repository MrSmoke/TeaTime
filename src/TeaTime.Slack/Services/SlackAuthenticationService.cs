namespace TeaTime.Slack.Services;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Client;
using Common.Abstractions;
using Models.OAuth;

internal class SlackAuthenticationService(
    ISlackApiClient slackApiClient,
    IUrlGenerator urlGenerator,
    IOptionsMonitor<SlackOAuthOptions> optionsMonitor)
    : ISlackAuthenticationService
{
    private static readonly StringValues Scopes = string.Join(',', Constants.OAuthScopes);

    public bool OAuthEnabled() => optionsMonitor.CurrentValue.Enabled;

    public string BuildAuthorizeUrl()
    {
        var oauth = GetOptions();

        const string baseUrl = "https://slack.com/oauth/v2/authorize";

        return QueryHelpers.AddQueryString(baseUrl, new KeyValuePair<string, StringValues>[]
        {
            new("client_id", oauth.ClientId),
            new("scope", Scopes),
            new("redirect_uri", GetRedirectUrl())
        });
    }

    public Task<OAuthTokenResponse> GetOAuthTokenAsync(string code, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        var oauth = GetOptions();

        return slackApiClient.GetOAuthTokenAsync(new OAuthTokenRequest
        {
            ClientId = oauth.ClientId ?? throw new InvalidOperationException("ClientId missing"),
            ClientSecret = oauth.ClientSecret ?? throw new InvalidOperationException("ClientSecret missing"),
            RedirectUri = GetRedirectUrl(),
            Code = code
        });
    }

    private string GetRedirectUrl() => urlGenerator.CreateAbsoluteUrlByName(Constants.RouteNames.OauthCallback);

    private SlackOAuthOptions GetOptions()
    {
        var options = optionsMonitor.CurrentValue;
        if (!options.Enabled)
            throw new InvalidOperationException("Oauth is not enabled");

        if (!options.IsValid())
            throw new InvalidOperationException("Oauth is not configured");

        return options;
    }
}
