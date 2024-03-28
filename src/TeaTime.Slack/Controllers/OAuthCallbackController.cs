namespace TeaTime.Slack.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Models.Responses;
    using Models.ViewModels;
    using Services;
    using static Constants;

    public class OAuthCallbackController(
        ISlackAuthenticationService slackAuthenticationService,
        ILogger<OAuthCallbackController> logger,
        ISlackService slackService)
        : Controller
    {
        [HttpGet("slack/callback", Name = RouteNames.OauthCallback)]
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

            OAuthTokenResponse response;

            try
            {
                // If we want to use the access token we'll need to store it
                response = await slackAuthenticationService.GetOAuthTokenAsync(code);

                // Check response is successful or not (according to slack)
                if (!response.IsSuccess)
                {
                    logger.LogError("Failed to install into slack channel. Error: {ErrorCode}", response.Error);

                    // set the error code for the view
                    return View(OAuthCallbackViewModel.Error("slack:" + response.Error));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get OAuth token");
                return View(OAuthCallbackViewModel.Error("unknown"));
            }

            try
            {
                await slackService.InstallAsync(response);
                return View(OAuthCallbackViewModel.Ok(response.Team.Name, response.IncomingWebhook.Channel));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to install TeaTime into slack");
                return View(OAuthCallbackViewModel.Error("installation_error"));
            }
        }
    }
}
