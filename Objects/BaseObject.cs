#region
using Newtonsoft.Json;

#endregion

namespace BlockCypher.Objects {
    public class BaseObject {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("errors")]
        public ErrorObj[] Errors { get; set; }

        public bool IsError {
            get { return !string.IsNullOrEmpty(Error) || Errors != null; }
        }
    }

    public class ErrorObj {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}