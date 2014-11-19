#region
using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class AddressBalance {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance"), JsonConverter(typeof (SatoshiConverter))]
        public Satoshi Balance { get; set; }

        [JsonProperty("final_balance"), JsonConverter(typeof (SatoshiConverter))]
        public Satoshi FinalBalance { get; set; }

        [JsonProperty("final_n_tx")]
        public int FinalTx { get; set; }

        [JsonProperty("n_tx")]
        public int TotalTransactions { get; set; }

        [JsonProperty("txrefs")]
        public IList<TxReference> Transactions { get; set; }

        [JsonProperty("tx_url")]
        public string TxUrl { get; set; }

        [JsonProperty("unconfirmed_balance")]
        public int UnconfirmedBalance { get; set; }

        [JsonProperty("unconfirmed_n_tx")]
        public int UnconfirmedTx { get; set; }
    }
}