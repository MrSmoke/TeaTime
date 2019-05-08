namespace TeaTime.Slack.Models.Responses
{
    public class OAuthTokenResponse
    {
        public string AccessToken { get; set; }
        public string Scope { get; set; }
        public string TeamName { get; set; }
        public string TeamId { get; set; }
    }
}
