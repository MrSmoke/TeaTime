namespace TeaTime.Slack
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal static class SlackJsonSerializer
    {
        internal static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            // Slack treats optional fields as absent, not null
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        internal static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, Options);
        }

        internal static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, Options);
        }
    }
}
