namespace TeaTime.Slack.Models.Requests
{
    public class OAuthTokenRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Code { get; set; }
        public string RedirectUri { get; set; }
    }
}
