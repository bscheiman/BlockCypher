#region
using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class UnsignedTransaction : BaseObject {
        [JsonProperty("pubkeys")]
        public IList<string> PubKeys { get; set; }

        [JsonProperty("signatures")]
        public IList<string> Signatures { get; set; }

        [JsonProperty("tosign")]
        public IList<string> ToSign { get; set; }

        [JsonProperty("tx")]
        public UnsignedInner Transactions { get; set; }
    }
}
