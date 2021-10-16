using Newtonsoft.Json;

namespace Simple.Extensions.Json
{
    public static class JsonSerializer
    {
        public static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public static T ToObject<T>(this string json, JsonSerializerSettings settings = null)
        {
            return json == null ? default : JsonConvert.DeserializeObject<T>(json, settings ?? DefaultSettings);
        }

        public static string ToJson(this object obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings ?? DefaultSettings);
        }
    }
}
