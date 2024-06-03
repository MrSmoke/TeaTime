namespace TeaTime.Slack.Client
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Models.OAuth;
    using Models.SlashCommands;

    internal class SlackApiClient(HttpClient httpClient) : ISlackApiClient
    {
        public Task PostResponseAsync(string callbackUrl, SlashCommandResponse response)
        {
            return PostAsync(callbackUrl, response);
        }

        public async Task<OAuthTokenResponse> GetOAuthTokenAsync(OAuthTokenRequest request)
        {
            var formData = new Dictionary<string, string>
            {
                {"client_id", request.ClientId},
                {"client_secret", request.ClientSecret},
                {"code", request.Code},
                {"grant_type", "authorization_code"},
                {"redirect_uri", request.RedirectUri},
            };

            using var content = new FormUrlEncodedContent(formData);
            using var response = await httpClient.PostAsync("https://slack.com/api/oauth.v2.access", content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<OAuthTokenResponse>() ??
                   throw new JsonException("Response returned null when a type was expected");
        }

        private async Task<HttpStatusCode> PostAsync(string url, object body)
        {
            using var response = await httpClient.PostAsJsonAsync(url, body);

            //todo: log errors
            return response.StatusCode;
        }
    }
}
