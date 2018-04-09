namespace TeaTime.Slack.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class OAuthCallbackController : Controller
    {
        [Route("slack/callback")]
        public IActionResult Callback()
        {
            //TODO:
            return Ok();
        }
    }
}
