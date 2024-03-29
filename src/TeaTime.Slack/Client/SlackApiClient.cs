namespace TeaTime.Slack.Client;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Models.OAuth;
using Models.SlashCommands;

internal class SlackApiClient(HttpClient httpClient) : ISlackApiClient
{
    public Task PostResponseAsync(string callbackUrl, SlashCommandResponse response,
        CancellationToken cancellationToken = default)
    {
        return PostAsync(callbackUrl, response, cancellationToken);
    }

    public Task PostMessageAsync(string webhookUrl, SlackMessage message,
        CancellationToken cancellationToken = default)
    {
        return PostAsync(webhookUrl, message, cancellationToken);
    }

    public async Task<OAuthTokenResponse> GetOAuthTokenAsync(OAuthTokenRequest request)
    {
        var formData = new Dictionary<string, string>
        {
            { "client_id", request.ClientId },
            { "client_secret", request.ClientSecret },
            { "code", request.Code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", request.RedirectUri },
        };

        using var content = new FormUrlEncodedContent(formData);
        using var response = await httpClient.PostAsync("https://slack.com/api/oauth.v2.access", content);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OAuthTokenResponse>() ??
               throw new JsonException("Response returned null when a type was expected");
    }

    private async Task PostAsync<T>(string url, T body,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(url, body, cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        // todo: handle errors
    }
}
