﻿namespace Minder.Service.Models.User {

    public class ChangePasswordReq {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}