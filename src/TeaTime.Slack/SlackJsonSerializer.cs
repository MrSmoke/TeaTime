namespace TeaTime.Slack
{
    using System.Text.Json;

    internal static class SlackJsonSerializer
    {
        private static readonly JsonSerializerOptions JsonSerializerSettings = new();

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
