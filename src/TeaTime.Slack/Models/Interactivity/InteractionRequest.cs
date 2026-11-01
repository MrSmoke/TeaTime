namespace TeaTime.Slack.Models.Interactivity;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Form body Slack posts to the interactivity request URL
/// </summary>
public class InteractionRequest
{
    [FromForm(Name = "payload")]
    public string? PayloadJson { get; set; }
}
