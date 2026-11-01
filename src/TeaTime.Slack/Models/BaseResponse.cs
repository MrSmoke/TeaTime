namespace TeaTime.Slack.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Common Web API envelope. See https://docs.slack.dev/apis/web-api/#responses
/// </summary>
public abstract class BaseResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("warning")]
    public string? Warning { get; set; }

    public bool IsSuccess => Ok;
}
