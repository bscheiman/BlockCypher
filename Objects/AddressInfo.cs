#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class AddressInfo : BaseObject {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("private")]
        public string Private { get; set; }

        [JsonProperty("public")]
        public string Public { get; set; }

        public AddressInfo() {
        }

        public AddressInfo(string address, string @private, string @public) {
            Address = address;
            Private = @private;
            Public = @public;
        }
    }
}