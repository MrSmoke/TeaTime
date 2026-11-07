namespace TeaTime.Slack.Models.SlashCommands;

using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter<ResponseType>))]
public enum ResponseType
{
    // Visible to everyone in the channel
    [JsonStringEnumMemberName("in_channel")]
    Channel,

    // Ephemeral, only visible to the user who triggered it
    [JsonStringEnumMemberName("ephemeral")]
    User
}
