#region
using System;
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class TxReference {
        [JsonProperty("block_height")]
        public int BlockHeight { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("confirmed")]
        public DateTime Confirmed { get; set; }

        [JsonProperty("double_spend")]
        public bool DoubleSpend { get; set; }

        [JsonProperty("spent")]
        public bool Spent { get; set; }

        [JsonProperty("spent_by")]
        public string SpentBy { get; set; }

        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }

        [JsonProperty("tx_input_n")]
        public int TxInputN { get; set; }

        [JsonProperty("tx_output_n")]
        public int TxOutputN { get; set; }

        [JsonProperty("value"), JsonConverter(typeof(SatoshiConverter))]
        public Satoshi Value { get; set; }
    }
}