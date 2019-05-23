namespace TeaTime.Slack.Client
{
    using System.Threading.Tasks;
    using Models.Responses;
    using Models.Requests;

    public interface ISlackApiClient
    {
        Task PostResponseAsync(string callbackUrl, SlashCommandResponse response);
        Task<OAuthTokenResponse> GetOAuthTokenAsync(OAuthTokenRequest request);
    }
}