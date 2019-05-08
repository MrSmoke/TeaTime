namespace TeaTime.Slack.Client
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Models.Responses;
    using Models.Requests;

    internal class SlackApiClient : ISlackApiClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

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
                {"redirect_uri", request.RedirectUri.ToString()},
            };

            using (var content = new FormUrlEncodedContent(formData))
            using (var response = await _httpClient.PostAsync("https://slack.com/api/oauth.access", content))
            {
                //todo: log errors
                return Deserialize<OAuthTokenResponse>(await response.Content.ReadAsStringAsync());
            }
        }

        internal async Task<HttpStatusCode> PostAsync(string url, object body)
        {
            var json = Serialize(body);

            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            using (var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false))
            {
                //todo: log errors
                return response.StatusCode;
            }
        }

        private static string Serialize(object data)
        {
            return SlackJsonSerializer.Serialize(data);
        }

        private static T Deserialize<T>(string json)
        {
            return SlackJsonSerializer.Deserialize<T>(json);
        }
    }
}
