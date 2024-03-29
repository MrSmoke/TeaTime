namespace TeaTime.Slack.Client;

public class SlackMessage(string? text = null)
{
    public string? Text { get; init; } = text;
}
