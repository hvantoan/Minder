using Newtonsoft.Json;

namespace Minder.Service.Models.Auth {

    public class LoginRes {
        public string Token { get; set; } = null!;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? RefreshToken { get; set; }

        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}