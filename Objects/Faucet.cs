#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class Faucet : BaseObject {
        [JsonProperty("tx_ref")]
        public string TxReference { get; set; }
    }
}