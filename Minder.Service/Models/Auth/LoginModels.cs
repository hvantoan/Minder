using Newtonsoft.Json;

namespace Minder.Services.Models.Auth {

    public class LoginRequest {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponse {
        public string Token { get; set; } = null!;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string? RefreshToken { get; set; }

        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public long ExpiredTime { get; set; }
    }

    public class LoginGoogleRequest {
        public string ExternalToken { get; set; } = null!;
        public string ExternalId { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}