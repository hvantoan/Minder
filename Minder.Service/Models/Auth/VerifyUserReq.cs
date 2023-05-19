namespace Minder.Service.Models.Auth {

    public class VerifyUserReq {
        public string Code { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}