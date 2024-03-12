namespace TeaTime.Slack;

using System.Threading;
using System.Threading.Tasks;
using Models.Responses;

public interface ISlackAuthenticationService
{
    public bool OAuthEnabled();
    string BuildAuthorizeUrl();
    Task<OAuthTokenResponse> GetOAuthTokenAsync(string code, CancellationToken cancellationToken = default);
}
