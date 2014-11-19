#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class AddressInfo {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("private")]
        public string Private { get; set; }

        [JsonProperty("public")]
        public string Public { get; set; }
    }
}