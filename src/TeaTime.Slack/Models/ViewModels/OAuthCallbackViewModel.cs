namespace TeaTime.Slack.Models.ViewModels;

public class OAuthCallbackViewModel(bool success)
{
    public bool Success { get; } = success;
    public string? TeamName { get; private init; }
    public string? ChannelName { get; private init; }
    public string? ErrorCode { get; private init; }

    public static OAuthCallbackViewModel Ok(string teamName, string? channelName) => new(true)
    {
        TeamName = teamName,
        ChannelName = channelName
    };

    public static OAuthCallbackViewModel Error(string errorCode) => new(false)
    {
        ErrorCode = errorCode
    };
}
