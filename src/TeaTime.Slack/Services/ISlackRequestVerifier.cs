namespace TeaTime.Slack.Services;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public interface ISlackRequestVerifier
{
    bool IsEnabled();
    Task<bool> VerifyAsync(HttpRequest request, CancellationToken cancellationToken = default);
}
