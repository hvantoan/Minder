namespace Minder.Services.Models.User {

    public class ChangePasswordRequest {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}