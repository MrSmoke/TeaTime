namespace TeaTime.Slack.Models.BlockKit;

using System.Text.Json.Serialization;

/// <summary>
/// Button colour. Leave the style null for the default grey.
/// See https://docs.slack.dev/reference/block-kit/block-elements/button-element
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ButtonStyle>))]
public enum ButtonStyle
{
    // Green
    [JsonStringEnumMemberName("primary")]
    Primary,

    // Red
    [JsonStringEnumMemberName("danger")]
    Danger
}
