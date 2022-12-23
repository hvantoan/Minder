using Minder.Database.Enums;
using Minder.Services.Models;
using Minder.Services.Models.User;

namespace Minder.Service.Models.User {

    public class ListUserRequest : BaseListRequest { }

    public class GetUserRequest : BaseGetRequest { }

    public class CreateUserRequest {
        public UserDto User { get; set; } = null!;
        public ERoleType Role { get; set; }
    }

    public class ChangePasswordRequest {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class ResetPasswordRequest {
        public string Password { get; set; } = null!;
    }
}