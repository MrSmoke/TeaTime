namespace TeaTime.Slack.Models.Requests
{
    using Microsoft.AspNetCore.Mvc;

    /*
    token=gIkuvaNzQIHg97ATvDxqgjtO
    team_id=T0001
    team_domain=example
    enterprise_id=E0001
    enterprise_name=Globular%20Construct%20Inc
    channel_id=C2147483705
    channel_name=test
    user_id=U2147483697
    user_name=Steve
    command=/weather
    text=94070
    response_url=https://hooks.slack.com/commands/1234/5678
    */

    public class SlashCommand : IVerifiableRequest
    {
        public string? Token { get; set; }

        [FromForm(Name = "team_id")]
        public string TeamId { get; set; }
        [FromForm(Name = "team_domain")]
        public string TeamDomain { get; set; }

        [FromForm(Name = "enterprise_id")]
        public string EnterpriseId { get; set; }
        [FromForm(Name = "enterprise_name")]
        public string EnterpriseName { get; set; }

        [FromForm(Name = "channel_id")]
        public string ChannelId { get; set; }
        [FromForm(Name = "channel_name")]
        public string ChannelName { get; set; }

        [FromForm(Name = "user_id")]
        public string UserId { get; set; }
        [FromForm(Name = "user_name")]
        public string UserName { get; set; }

        public string Command { get; set; }
        public string Text { get; set; }
        [FromForm(Name = "response_url")]
        public string ResponseUrl { get; set; }
    }
}
