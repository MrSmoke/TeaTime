namespace TeaTime.Slack.Client
{
    using System.Threading.Tasks;
    using Models.OAuth;
    using Models.SlashCommands;

    public interface ISlackApiClient
    {
        Task PostResponseAsync(string callbackUrl, SlashCommandResponse response);
        Task<OAuthTokenResponse> GetOAuthTokenAsync(OAuthTokenRequest request);
    }
}
