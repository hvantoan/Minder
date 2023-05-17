﻿namespace Minder.Services.Resources {

    public static class Messages {
        public static ResponseMessage SuccessMessage { get; } = new() { Code = 0, Message = "Thành công." };

        public static class Auth {
            public static ResponseMessage Auth_NotFound { get; } = new() { Code = 1, Message = "Người dùng không tồn tại." };
            public static ResponseMessage Auth_Inactive { get; } = new() { Code = 2, Message = "Người dùng không hoạt động." };
            public static ResponseMessage Auth_IncorrectPassword { get; } = new() { Code = 3, Message = "Sai mật khẩu." };
            public static ResponseMessage Auth_NoPermission { get; } = new() { Code = 4, Message = "Bạn không có quyền đăng nhập vào hệ thống. Vui lòng liên hệ quản trị viên để được hổ trợ." };
            public static ResponseMessage Auth_IncorresctOTP { get; } = new() { Code = 5, Message = "Mã xác nhận không chính xác." };
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

        public static class Team {
            public static ResponseMessage Team_CodeRequired { get; } = new() { Code = 30, Message = "Mã đội bóng có từ  2 - 4 ký tự và không được để trống." };
            public static ResponseMessage Team_CodeExited { get; } = new() { Code = 31, Message = "Tên viết tắt của đội bóng đã tồn tại" };
            public static ResponseMessage Team_NameRequired { get; } = new() { Code = 32, Message = "Tên đội bóng có từ 2 - 32 ký tự và không được để trống." };
            public static ResponseMessage Team_AvatarRequired { get; } = new() { Code = 33, Message = "Ảnh đội bóng không được để trống." };
            public static ResponseMessage Team_DescriptionRequired { get; } = new() { Code = 34, Message = "Mô tả có độ dài ít hơn 80 ký tự." };
            public static ResponseMessage Team_NotFound { get; } = new() { Code = 35, Message = "Đội bóng không tồn tại." };
            public static ResponseMessage Team_IsOwner { get; } = new() { Code = 36, Message = "Bạn đã có đội bóng của mình rồi." };
            public static ResponseMessage Team_NoPermistion { get; } = new() { Code = 37, Message = "Bạn không có đủ quyền." };
            public static ResponseMessage Team_NotOut { get; } = new() { Code = 38, Message = "Bạn không thể rời đội khi đang là chủ sở hửu." };
            public static ResponseMessage Team_NotInTeam { get; } = new() { Code = 38, Message = "Người dùng không ở trong đội." };
            public static ResponseMessage Team_NotInviteYourself { get; } = new() { Code = 39, Message = "Không thể mời chính mình." };
        }

        public static class File {
            public static ResponseMessage File_NotFound { get; } = new() { Code = 50, Message = "Tệp không không tồn tại." };
            public static ResponseMessage File_Error { get; } = new() { Code = 51, Message = "Lỗi trong quá trình sử lý." };
        }

        public static class Invite {
            public static ResponseMessage Invite_IsExited { get; } = new() { Code = 60, Message = "Đã gửi lời mời cho người dùng này." };
            public static ResponseMessage Invite_NotFound { get; } = new() { Code = 61, Message = "Lời mời không tồn tại." };
            public static ResponseMessage Invite_TeamNotFound { get; } = new() { Code = 62, Message = "Đội không tồn tại." };
            public static ResponseMessage Invite_UserNotFound { get; } = new() { Code = 63, Message = "Người dùng không tồn tại." };
            public static ResponseMessage Invite_IsSended { get; } = new() { Code = 64, Message = "Bạn đã gửi lời mời xin vào đội này." };
        }

        public static class Stadium {
            public static ResponseMessage Stadium_NotFound { get; } = new() { Code = 70, Message = "Sân bóng này không tồn tại." };
            public static ResponseMessage Stadium_CodeRequired { get; } = new() { Code = 71, Message = "Mã sân bóng có từ  2 - 4 ký tự và không được để trống." };
            public static ResponseMessage Stadium_NameRequired { get; } = new() { Code = 72, Message = "Tên sân bóng không được để trống." };
            public static ResponseMessage Stadium_CodeExited { get; } = new() { Code = 72, Message = "Mã sân đã tồn tại." };
            public static ResponseMessage Stadium_InitFounded { get; } = new() { Code = 73, Message = "Dữ liệu mặc định đã được thêm vào hệ thống." };
        }

        public static class Email {
            public static ResponseMessage User_NameRequired { get; } = new() { Code = 80, Message = "Gửi email thất bại." };
        }

        public static class Conversation {
            public static ResponseMessage Conversation_NameRequire { get; } = new() { Code = 90, Message = "Tên là bắt buộc." };
            public static ResponseMessage Conversation_MinParticipant { get; } = new() { Code = 90, Message = "Hội thoại tối thiểu phải có 2 người." };
            public static ResponseMessage Conversation_NotFound { get; } = new() { Code = 90, Message = "Đoạn hội thoại không tồn tại." };
        }

        public static class Match {
            public static ResponseMessage HostTeam_NotFount { get; } = new() { Code = 100, Message = "Đội của bạn không tồn tại." };
            public static ResponseMessage OpposingTeam_NotFount { get; } = new() { Code = 101, Message = "Đội đối thủ không tồn tại không tồn tại." };
            public static ResponseMessage Match_NotFount { get; } = new() { Code = 102, Message = "Trận đấu không tồn tại." };
            public static ResponseMessage Match_TeamIdRequire { get; } = new() { Code = 103, Message = "TeamId không được để trống." };
        }

        public static class System {
            public static ResponseMessage System_Error { get; } = new() { Code = 90, Message = "Lỗi hệ thống." };
        }
    }

    public class ResponseMessage {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public static class ValidateMessage {
        public const string IsValid = "Hợp lệ.";
        public const string IsNotValid = "Tên đăng nhập đã tồn tại.";
    }
}