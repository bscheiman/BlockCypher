#region
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace BlockCypher.Objects {
    public class BaseObject {
        [JsonProperty("error")]
        public string Error { get; set; }

        public string[] Errors {
            get {
                if (Extra == null || !Extra.ContainsKey("errors"))
                    return null;

                return Extra["errors"].Select(a => a["error"].ToString()).ToArray();
            }
        }

        [JsonExtensionData]
        private IDictionary<string, JToken> Extra { get; set; }

        public bool IsError {
            get { return !string.IsNullOrEmpty(Error) || Errors != null; }
        }
    }
}