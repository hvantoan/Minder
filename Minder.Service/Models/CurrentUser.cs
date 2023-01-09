namespace Minder.Service.Models {

    public class CurrentUser {
        public string UserId { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? OTP { get; set; }
    }
}