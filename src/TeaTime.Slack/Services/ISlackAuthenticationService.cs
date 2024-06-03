namespace TeaTime.Slack.Services;

using System.Threading;
using System.Threading.Tasks;
using Models.OAuth;

public interface ISlackAuthenticationService
{
    public bool OAuthEnabled();
    string BuildAuthorizeUrl();
    Task<OAuthTokenResponse> GetOAuthTokenAsync(string code, CancellationToken cancellationToken = default);
}
