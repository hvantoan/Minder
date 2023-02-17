namespace Minder.Service.Models.Auth {

    public class LoginReq {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}