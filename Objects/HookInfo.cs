#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class HookInfo : BaseObject {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("filter")]
        public string Filter { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}