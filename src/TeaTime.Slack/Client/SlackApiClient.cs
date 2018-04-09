namespace TeaTime.Slack.Client
{
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Models.Responses;

    internal class SlackApiClient : ISlackApiClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        internal async Task<HttpStatusCode> PostAsync(string url, object body)
        {
            var json = Serialize(body);

            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                using (var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false))
                {
                    //todo: log errors
                    return response.StatusCode;
                }
            }
        }

        public Task PostResponseAsync(string callbackUrl, SlashCommandResponse response)
        {
            return PostAsync(callbackUrl, response);
        }

        private static string Serialize(object data)
        {
            return SlackJsonSerializer.Serialize(data);
        }
    }
}
