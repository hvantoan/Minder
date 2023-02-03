using Newtonsoft.Json;

namespace Minder.Service.Models.Auth {

    public class VerifyResponse {
        public bool Status { get; set; } = true;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? Token { get; set; }
    }
}