﻿namespace Minder.Service.Models.Auth {
    public class SignUpRequest {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
