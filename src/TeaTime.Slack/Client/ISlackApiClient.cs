namespace TeaTime.Slack.Client;

using System.Threading;
using System.Threading.Tasks;
using Models.OAuth;
using Models.SlashCommands;

public interface ISlackApiClient
{
    Task PostResponseAsync(string callbackUrl, SlashCommandResponse response,
        CancellationToken cancellationToken = default);

    Task PostMessageAsync(string webhookUrl, SlackMessage message, CancellationToken cancellationToken = default);
    Task<OAuthTokenResponse> GetOAuthTokenAsync(OAuthTokenRequest request);
}
