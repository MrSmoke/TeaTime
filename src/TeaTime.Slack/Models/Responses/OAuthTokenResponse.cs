namespace TeaTime.Slack.Models.Responses
{
    using Newtonsoft.Json;

    public class OAuthTokenResponse : BaseResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("team_name")]
        public string TeamName { get; set; }

        [JsonProperty("team_id")]
        public string TeamId { get; set; }
    }
}
