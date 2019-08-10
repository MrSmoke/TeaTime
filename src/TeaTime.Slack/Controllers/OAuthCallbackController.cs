namespace TeaTime.Slack.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Client;
    using Common;
    using Configuration;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.Requests;
    using Models.ViewModels;

    public class OAuthCallbackController : Controller
    {
        private readonly ISlackApiClient _slackApiClient;
        private readonly IOptionsMonitor<SlackOptions> _optionsAccessor;
        private readonly ILogger<OAuthCallbackController> _logger;
        private readonly IDistributedHash _hash;

        public OAuthCallbackController(ISlackApiClient slackApiClient, IOptionsMonitor<SlackOptions> optionsAccessor,
            ILogger<OAuthCallbackController> logger, IDistributedHash hash)
        {
            _slackApiClient = slackApiClient;
            _optionsAccessor = optionsAccessor;
            _logger = logger;
            _hash = hash;
        }

        [Route("slack/callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return NotFound();

            _logger.LogInformation("OAuth callback called with code {Code}", code);

            var options = _optionsAccessor.CurrentValue.OAuth;

            if (options == null || !options.IsValid())
            {
                _logger.LogWarning("Ignoring OAuth callback. No OAuth settings set");

                return View(OAuthCallbackViewModel.Error("not_configured"));
            }

            var errorCode = "unknown";

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

                    var fields = new List<HashEntry>
                    {
                        new HashEntry("team_name", response.TeamName),
                        new HashEntry("access_token", response.AccessToken),
                        new HashEntry("install_time", DateTime.UtcNow.ToString("O"))
                    };

                    // Add config for webhook if available
                    if (response.IncomingWebhook != null)
                    {
                        fields.Add(new HashEntry($"webhook:{response.IncomingWebhook.ChannelId}:url",
                            response.IncomingWebhook.Url));
                    }

                    await _hash.SetAsync("slack:" + response.TeamId, fields);

                    return View(OAuthCallbackViewModel.Ok(response.TeamName));
                }

                _logger.LogError("Failed to install into slack channel. Error: {ErrorCode}", response.Error);

                // set the error code for the view
                errorCode = "slack:" + response.Error;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get OAuth token");
            }

            return View(OAuthCallbackViewModel.Error(errorCode));
        }
    }
}
