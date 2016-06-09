#region
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
}
