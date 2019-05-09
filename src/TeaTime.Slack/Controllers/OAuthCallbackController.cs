namespace TeaTime.Slack.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Client;
    using Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.Requests;

    public class OAuthCallbackController : Controller
    {
        private readonly ISlackApiClient _slackApiClient;
        private readonly IOptionsMonitor<SlackOptions> _optionsAccessor;
        private readonly ILogger<OAuthCallbackController> _logger;

        public OAuthCallbackController(ISlackApiClient slackApiClient, IOptionsMonitor<SlackOptions> optionsAccessor, ILogger<OAuthCallbackController> logger)
        {
            _slackApiClient = slackApiClient;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
        }

        [Route("slack/callback")]
        public async Task<IActionResult> Callback(string code)
        {
            _logger.LogInformation("OAuth callback called with code {Code}", code);

            var options = _optionsAccessor.CurrentValue.OAuth;

            if (options == null || !options.IsValid())
            {
                _logger.LogWarning("Ignoring OAuth callback. No OAuth settings set");

                return Content("Cannot install. No OAuth settings set");
            }

            try
            {
                // If we want to use the access token we'll need to store it
                var response = await _slackApiClient.GetOAuthTokenAsync(new OAuthTokenRequest
                {
                    ClientId = options.ClientId,
                    ClientSecret = options.ClientSecret,
                    RedirectUri = options.RedirectUri,
                    Code = code
                });

                if (response.IsSuccess)
                {
                    _logger.LogInformation("TeaTime installed in team {TeamId} ({TeamName}) with scope {Scope}",
                        response.TeamId, response.TeamName, response.Scope);

                    return Content("Installed!");
                }

                _logger.LogError("Failed to install into slack channel. Error: {ErrorCode}", response.Error);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get OAuth token");
            }

            return Content("Failed to install :(");
        }
    }
}
