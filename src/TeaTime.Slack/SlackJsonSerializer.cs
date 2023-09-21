namespace TeaTime.Slack
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal static class SlackJsonSerializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        internal static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, JsonSerializerSettings);
        }

        internal static T? Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}
