namespace TeaTime.Slack.Controllers
{
    using System.Threading.Tasks;
    using Client;
    using Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models.Requests;

    public class OAuthCallbackController : Controller
    {
        private readonly ISlackApiClient _slackApiClient;
        private readonly IOptionsMonitor<SlackOptions> _optionsAccessor;

        public OAuthCallbackController(ISlackApiClient slackApiClient, IOptionsMonitor<SlackOptions> optionsAccessor)
        {
            _slackApiClient = slackApiClient;
            _optionsAccessor = optionsAccessor;
        }

        [Route("slack/callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var options = _optionsAccessor.CurrentValue.OAuth;

            if (options == null || !options.IsValid())
                return Content("Cannot install. No OAuth settings set");

            try
            {
                // If we want to use the access token we'll need to store it
                await _slackApiClient.GetOAuthTokenAsync(new OAuthTokenRequest
                {
                    ClientId = options.ClientId,
                    ClientSecret = options.ClientSecret,
                    RedirectUri = options.RedirectUri,
                    Code = code
                });

                return Content("Installed!");
            }
            catch
            {
                return Content("Failed to install :(");
            }
        }
    }
}
