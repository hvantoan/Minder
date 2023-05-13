using System.ComponentModel;

namespace Minder.Database.Enums {

    public enum EMatch {

        [Description("Chuẩn bị")]
        Prepare,

        [Description("Hoàn thành")]
        Complete,

        [Description("Hủy")]
        Cancel,
    }
}