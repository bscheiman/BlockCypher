#region
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class Input {
        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }

        [JsonProperty("output_index")]
        public long OutputIndex { get; set; }

        [JsonProperty("output_value")]
        public Satoshi OutputValue { get; set; }

        [JsonProperty("prev_hash")]
        public string PrevHash { get; set; }

        [JsonProperty("script")]
        public string Script { get; set; }

        [JsonProperty("script_type")]
        public string ScriptType { get; set; }

        [JsonProperty("sequence")]
        public object Sequence { get; set; }
    }

    public class Output {
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

    public class Transaction {
        [JsonProperty("addresses")]
        public IList<string> Addresses { get; set; }

        [JsonProperty("block_hash")]
        public string BlockHash { get; set; }

        [JsonProperty("block_height")]
        public long BlockHeight { get; set; }

        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty("confirmed")]
        public DateTime Confirmed { get; set; }

        [JsonProperty("double_spend")]
        public bool DoubleSpend { get; set; }

        [JsonProperty("fees")]
        public Satoshi Fees { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("inputs")]
        public IList<Input> Inputs { get; set; }

        [JsonProperty("lock_time")]
        public int LockTime { get; set; }

        [JsonProperty("outputs")]
        public IList<Output> Outputs { get; set; }

        [JsonProperty("preference")]
        public string Preference { get; set; }

        [JsonProperty("received")]
        public string Received { get; set; }

        [JsonProperty("relayed_by")]
        public string RelayedBy { get; set; }

        [JsonProperty("total")]
        public Satoshi Total { get; set; }

        [JsonProperty("ver")]
        public int Ver { get; set; }

        [JsonProperty("vin_sz")]
        public int VinSz { get; set; }

        [JsonProperty("vout_sz")]
        public int VoutSz { get; set; }
    }
}