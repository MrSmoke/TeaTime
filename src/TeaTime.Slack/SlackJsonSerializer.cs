namespace TeaTime.Slack
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal static class SlackJsonSerializer
    {
        private static readonly JsonSerializerOptions JsonSerializerSettings = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        internal static string Serialize(object obj)
        {
            return JsonSerializer.Serialize(obj, JsonSerializerSettings);
        }

        internal static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerSettings);
        }
    }
}
