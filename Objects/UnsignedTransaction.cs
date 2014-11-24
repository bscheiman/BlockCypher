#region
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class UnsignedInput {
        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }

        [JsonProperty("output_index")]
        public int OutputIndex { get; set; }

        [JsonProperty("output_value")]
        public int OutputValue { get; set; }

        [JsonProperty("prev_hash")]
        public string PrevHash { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("script_type")]
        public string ScriptType { get; set; }
    }

    public class UnsignedOutput {
        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("script_type")]
        public string ScriptType { get; set; }

        [JsonProperty("spent_by")]
        public string SpentBy { get; set; }

        [JsonProperty("value")]
        public Satoshi Value { get; set; }
    }

    public class UnsignedInner {
        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }

        [JsonProperty("block_height")]
        public int BlockHeight { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("confirmed")]
        public DateTime Confirmed { get; set; }

        [JsonProperty("double_spend")]
        public bool DoubleSpend { get; set; }

        [JsonProperty("fees")]
        public int Fees { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("inputs")]
        public IList<UnsignedInput> Inputs { get; set; }

        [JsonProperty("lock_time")]
        public int LockTime { get; set; }

        [JsonProperty("outputs")]
        public IList<UnsignedOutput> Outputs { get; set; }

        [JsonProperty("preference")]
        public string Preference { get; set; }

        [JsonProperty("received")]
        public string Received { get; set; }

        [JsonProperty("relayed_by")]
        public string RelayedBy { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("ver")]
        public int Ver { get; set; }

        [JsonProperty("vin_sz")]
        public int VinSz { get; set; }

        [JsonProperty("vout_sz")]
        public int VoutSz { get; set; }
    }

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