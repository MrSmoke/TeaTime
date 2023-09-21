namespace TeaTime.Slack.Client
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Models.Responses;
    using Models.Requests;

    internal class SlackApiClient : ISlackApiClient
    {
        private readonly HttpClient _httpClient = new();

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
                {"redirect_uri", request.RedirectUri},
            };

            using var content = new FormUrlEncodedContent(formData);
            using var response = await _httpClient.PostAsync("https://slack.com/api/oauth.access", content);

            return await response.Content.ReadFromJsonAsync<OAuthTokenResponse>() ?? new OAuthTokenResponse();
        }

        private async Task<HttpStatusCode> PostAsync(string url, object body)
        {
            using var response = await _httpClient.PostAsJsonAsync(url, body);

            //todo: log errors
            return response.StatusCode;
        }
    }
}
