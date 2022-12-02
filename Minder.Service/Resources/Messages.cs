namespace Minder.Services.Resources {

    public static class Messages {



        public static class Auth {

            public static class Login {
                public const string Merchant_NotFound = "Cửa hàng không tồn tại.";
                public const string Merchant_Inactive = "Cửa hàng không hoạt động.";
                public const string Merchant_Expired = "Cửa hàng đã hết hạn.";

                public const string User_NotFound = "Người dùng không tồn tại.";
                public const string User_Inactive = "Người dùng không hoạt động.";
                public const string User_IncorrectPassword = "Sai mật khẩu.";
            }
        }

        public static class User {
            public const string User_NotFound = "Người dùng không tồn tại.";
            public static class CreateOrUpdate {
                public const string User_Existed = "Người dùng đã tồn tại.";
                public const string User_NotInactive = "Không thể dừng hoạt động với người quản trị.";
            }

            public static class ChangePassword {
                public const string User_IncorrentOldPassword = "Sai mật khẩu.";
            }

        }
        public static class Project {
            public const string Project_NotFound = "Dự án không tồn tại.";
            public const string ProjectDetail_NotFound = "Từ khóa không tồn tại.";
            public static class CreateOrUpdate {
                public const string Project_NameNotNullOrEmplty = "Tên không được để trống.";
                public const string Project_NameNotDuplicated = "Tên dự án đã được sử dụngs.";
                public const string User_NotInactive = "Không thể dừng hoạt động với người quản trị.";
            }
            public static class Get {
                public const string Project_NotFound = "Dự án không tồn tại.";

            }
        }

        public static class Seo {
        }

        public static class SearchContent {
            public const string SearchContent_IsEmpty = "Không có lịch sử tìm kiếm nào.";
            public const string SearchContent_NotFound = "Nội dung không tồn tại.";

        }
        public static class SearchIndex {
            public const string Index_KeyNotEmpty = "Từ khóa không được để trống.";
        }

    }
}