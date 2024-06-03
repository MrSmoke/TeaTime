namespace TeaTime.Slack.ViewModels;

public class OAuthCallbackViewModel(bool success)
{
    public bool Success { get; } = success;
    public string? TeamName { get; private init; }
    public string? ErrorCode { get; private init; }

    public static OAuthCallbackViewModel Ok(string teamName) => new(true)
    {
        TeamName = teamName,
    };

    public static OAuthCallbackViewModel Error(string errorCode) => new(false)
    {
        ErrorCode = errorCode
    };
}
