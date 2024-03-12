namespace TeaTime.Slack.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Collections;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.ViewModels;

    public class OAuthCallbackController(
        ISlackAuthenticationService slackAuthenticationService,
        ILogger<OAuthCallbackController> logger,
        IDistributedHash hash)
        : Controller
    {
        [Route("slack/callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return NotFound();

            logger.LogInformation("OAuth callback called with code {Code}", code);

            if (!slackAuthenticationService.OAuthEnabled())
            {
                logger.LogWarning("Ignoring OAuth callback. No OAuth settings set");
                return View(OAuthCallbackViewModel.Error("not_configured"));
            }

            try
            {
                // If we want to use the access token we'll need to store it
                var response = await slackAuthenticationService.GetOAuthTokenAsync(code);

                // Check response is successful or not (according to slack)
                if (!response.IsSuccess)
                {
                    logger.LogError("Failed to install into slack channel. Error: {ErrorCode}", response.Error);

                    // set the error code for the view
                    return View(OAuthCallbackViewModel.Error("slack:" + response.Error));
                }

                // Validate the response to make sure we have all the data
                if (!response.ValidateProperties())
                {
                    logger.LogWarning("Response is missing data {@Response}", response.ToLogObject());
                    return View(OAuthCallbackViewModel.Error("installation_error"));
                }

                logger.LogInformation("TeaTime installed in team {TeamId} ({TeamName}) with scope {Scope}",
                    response.TeamId, response.TeamName, response.Scope);

                var fields = new List<HashEntry>
                {
                    new("team_name", response.TeamName),
                    new("access_token", response.AccessToken),
                    new("install_time", DateTime.UtcNow.ToString("O"))
                };

                // Add config for webhook if available
                var incomingWebhook = response.IncomingWebhook;
                if (incomingWebhook != null)
                {
                    if (string.IsNullOrWhiteSpace(incomingWebhook.ChannelId) ||
                        string.IsNullOrWhiteSpace(incomingWebhook.Url))
                    {
                        logger.LogWarning("Incoming WebHook is missing data");
                    }
                    else
                    {

                        fields.Add(new HashEntry($"webhook:{incomingWebhook.ChannelId}:url", incomingWebhook.Url));
                    }
                }

                await hash.SetAsync("slack:" + response.TeamId, fields);

                return View(OAuthCallbackViewModel.Ok(response.TeamName));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get OAuth token");
                return View(OAuthCallbackViewModel.Error("unknown"));
            }
        }
    }
}
