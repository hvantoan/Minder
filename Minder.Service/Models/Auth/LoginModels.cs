namespace Minder.Services.Models.Auth {

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public long ExpiredTime { get; set; }
    }

    public class LoginGoogleRequest
    {
        public string ExternalToken { get; set; }
        public string ExternalId { get; set; }
        public string Email { get; set; }
    }
}