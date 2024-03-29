namespace TeaTime.Slack.Models.SlashCommands;

using System;
using Microsoft.AspNetCore.Mvc;

/*
token=gIkuvaNzQIHg97ATvDxqgjtO
&team_id=T0001
&team_domain=example
&enterprise_id=E0001
&enterprise_name=Globular%20Construct%20Inc
&channel_id=C2147483705
&channel_name=test
&user_id=U2147483697
&user_name=Steve
&command=/weather
&text=94070
&response_url=https://hooks.slack.com/commands/1234/5678
&trigger_id=13345224609.738474920.8088930838d88f008e0
&api_app_id=A123456
*/

public class SlashCommand
{
    /// <summary>
    /// The command that was entered to trigger this request
    /// </summary>
    public required string Command { get; set; }

    /// <summary>
    /// This is the part of the slash command after the command itself, and it can contain absolutely anything the user might decide to type
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// A temporary webhook URL that you can use to generate message responses.
    /// </summary>
    [FromForm(Name = "response_url")]
    public required string ResponseUrl { get; set; }

    /// <summary>
    /// The ID of the user who triggered the command.
    /// </summary>
    [FromForm(Name = "user_id")]
    public required string UserId { get; set; }

    /// <summary>
    /// The plain text name of the user who triggered the command.
    /// </summary>
    [FromForm(Name = "user_name")]
    [Obsolete("Slack is deprecating this property")]
    public string? UserName { get; set; }

    /// <summary>
    /// The ID of the channel the message was sent in
    /// </summary>
    [FromForm(Name = "channel_id")]
    public required string ChannelId { get; set; }

    /// <summary>
    /// The name of the channel the message was sent in
    /// </summary>
    [FromForm(Name = "channel_name")]
    public required string ChannelName { get; set; }
}
