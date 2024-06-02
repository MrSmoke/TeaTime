namespace TeaTime.Slack.Models;

public abstract class BaseResponse
{
    public bool Ok { get; set; }
    public string? Error { get; set; }

    public bool IsSuccess => Ok;
}
