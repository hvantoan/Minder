using System.ComponentModel;

namespace Minder.Database.Enums {

    public enum EMatch {

        [Description("Đợi xác nhận")]
        WaitingConfirm = 0,

        [Description("Chuẩn bị")]
        Prepare = 1,

        [Description("Hoàn thành")]
        Complete = 2,

        [Description("Hủy")]
        Cancel = 3,
    }
}