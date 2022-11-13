namespace Minder.Services.Models.User {

    public class ResetPasswordRequest {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}