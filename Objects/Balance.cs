#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class AddressBalance {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("balance")]
        public int Balance { get; set; }

        public decimal BalanceAsBTC {
            get { return (decimal) Balance / 100000000; }
        }

        [JsonProperty("final_balance")]
        public int FinalBalance { get; set; }

        public decimal FinalBalanceAsBTC {
            get { return (decimal) FinalBalance / 100000000; }
        }

        [JsonProperty("final_n_tx")]
        public int FinalTx { get; set; }

        [JsonProperty("n_tx")]
        public int Transactions { get; set; }

        [JsonProperty("tx_url")]
        public string TxUrl { get; set; }

        [JsonProperty("unconfirmed_balance")]
        public int UnconfirmedBalance { get; set; }

        [JsonProperty("unconfirmed_n_tx")]
        public int UnconfirmedTx { get; set; }
    }
}