namespace Minder.Services.Resources {

    public static class Messages {
        public static ResponseMessage SuccessMessage { get; } = new() { Code = 0, Message = "Thành công." };

        public static class Auth {
            public static ResponseMessage Auth_NotFound { get; } = new() { Code = 1, Message = "Người dùng không tồn tại." };
            public static ResponseMessage Auth_Inactive { get; } = new() { Code = 2, Message = "Người dùng không hoạt động." };
            public static ResponseMessage Auth_IncorrectPassword { get; } = new() { Code = 3, Message = "Sai mật khẩu." };
            public static ResponseMessage Auth_NoPermission { get; } = new() { Code = 4, Message = "Bạn không có quyền đăng nhập vào hệ thống. Vui lòng liên hệ quản trị viên để được hổ trợ." };
            public static ResponseMessage Auth_IncorresctOTP { get; } = new() { Code = 4, Message = "Mã xác nhận không chính xác." };
        }

        public static class User {
            public static ResponseMessage User_IncorrentOldPassword { get; } = new() { Code = 10, Message = "Mật khẩu cũ không chính xác." };
            public static ResponseMessage User_OTPIncorect { get; } = new() { Code = 11, Message = "Mã xác thực không chính xác." };
            public static ResponseMessage User_NotFound { get; } = new() { Code = 12, Message = "Người dùng không tồn tại." };
            public static ResponseMessage User_Existed { get; } = new() { Code = 13, Message = "Tên đăng nhập đã tồn tại." };
            public static ResponseMessage User_NotInactive { get; } = new() { Code = 14, Message = "Không thể dừng hoạt động với người quản trị." };
            public static ResponseMessage User_UsernameRequired { get; } = new() { Code = 15, Message = "Tên đăng nhập không được để trống." };
            public static ResponseMessage User_UsernameRequest { get; } = new() { Code = 16, Message = "Tên đăng nhập là email." };
            public static ResponseMessage User_PasswordRequired { get; } = new() { Code = 17, Message = "Mật khẩu không được để trống." };
            public static ResponseMessage User_PasswordRequest { get; } = new() { Code = 18, Message = "Mật khẩu phải có ít nhất 8 ký tự, không chứa khoản trắng." };
            public static ResponseMessage User_NameRequired { get; } = new() { Code = 19, Message = "Tên người dùng không được để trống." };
            public static ResponseMessage User_PhoneRequired { get; } = new() { Code = 20, Message = "Số điện thoại người dùng không được để trống." };
        }

        public static class Email {
            public static ResponseMessage User_NameRequired { get; } = new() { Code = 30, Message = "Gửi email thất bại." };
        }
    }

    public class ResponseMessage {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}