namespace TeaTime.Slack.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Collections;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.ViewModels;
    using Services;
    using static Constants;

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

            logger.LogInformation("OAuth callback called. Fetching oauth token...");

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
                    response.Team.Id, response.Team.Name, response.Scope);

                var hashKey = "slack:" + response.Team.Id;
                await hash.SetAsync(hashKey, new HashEntry[]
                {
                    new(FieldKeys.TeamName, response.Team.Name),
                    new(FieldKeys.AccessToken, response.AccessToken),
                    new(FieldKeys.InstallTime, DateTime.UtcNow.ToString("O"))
                });

                string? channelName = null;

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
                        var channelFields = new List<HashEntry>
                        {
                            new(FieldKeys.WebhookUrl, incomingWebhook.Url)
                        };

                        if (!string.IsNullOrEmpty(incomingWebhook.Channel))
                        {
                            channelFields.Add(new HashEntry(FieldKeys.ChannelName, incomingWebhook.Channel));
                            channelName = incomingWebhook.Channel;
                        }

                        var channelKey = hashKey + ":" + incomingWebhook.ChannelId;
                        await hash.SetAsync(channelKey, channelFields);
                    }
                }

                return View(OAuthCallbackViewModel.Ok(response.Team.Name, channelName));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get OAuth token");
                return View(OAuthCallbackViewModel.Error("unknown"));
            }
        }
    }
}
