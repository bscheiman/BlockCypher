#region
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
}
