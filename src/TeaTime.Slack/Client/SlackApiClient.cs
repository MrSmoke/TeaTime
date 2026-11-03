namespace TeaTime.Slack.Client;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.OAuth;
using Models.SlashCommands;

internal class SlackApiClient(HttpClient httpClient, ILogger<SlackApiClient> logger) : ISlackApiClient
{
    public Task PostResponseAsync(string callbackUrl, SlashCommandResponse response)
    {
        return PostAsync(callbackUrl, response);
    }

    public async Task<OAuthTokenResponse> GetOAuthTokenAsync(OAuthTokenRequest request)
    {
        var formData = new Dictionary<string, string>
        {
            { "code", request.Code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", request.RedirectUri },
        };

        // Slack recommends client credentials via HTTP Basic auth (RFC 6749 2.3.1)
        var credentials = Uri.EscapeDataString(request.ClientId) + ':' + Uri.EscapeDataString(request.ClientSecret);

        using var message = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/oauth.v2.access");

        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        message.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        message.Content = new FormUrlEncodedContent(formData);

        using var response = await httpClient.SendAsync(message);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OAuthTokenResponse>(SlackJsonSerializer.Options) ??
               throw new JsonException("Response returned null when a type was expected");
    }

    private async Task PostAsync(string url, object body)
    {
        try
        {
            using var response = await httpClient.PostAsJsonAsync(url, body, SlackJsonSerializer.Options);

            if (response.IsSuccessStatusCode)
                return;

            // Slack returns a short error code as the body. The URL is a credential, so it's not logged
            var error = await response.Content.ReadAsStringAsync();

            logger.LogWarning("Slack rejected response_url post with {StatusCode}: {SlackError}",
                (int)response.StatusCode, error);
        }
        catch (HttpRequestException e)
        {
            // Logged, not thrown, so a failed message doesn't fail the caller
            logger.LogError(e, "Failed to post to Slack response_url");
        }
        catch (TaskCanceledException e)
        {
            logger.LogError(e, "Timed out posting to Slack response_url");
        }
    }
}
