using System.ComponentModel;

namespace Minder.Database.Enums {

    public enum EMatch {

        [Description("Đợi xác nhận")]
        WaitingConfirm = 0,

        [Description("Chuẩn bị")]
        Prepare = 1,

        [Description("Đơi thành viên xác nhận")]
        WaitingMemberConfirm = 2,

        [Description("Hoàn thành")]
        Complete = 3,

        [Description("Hủy")]
        Cancel = 4,
    }
}