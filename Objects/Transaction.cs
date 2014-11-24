#region
using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class Transaction {
        [JsonProperty("inputs")]
        public IList<TxInput> Inputs { get; set; }

        [JsonProperty("outputs")]
        public IList<TxOutput> Outputs { get; set; }
    }
}