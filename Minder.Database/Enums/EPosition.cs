using System.ComponentModel;

namespace Minder.Database.Enums {

    public enum EPosition {

        [Description("Tiền đạo")]
        ST = 0,

        [Description("Trung Vệ")]
        CM = 1,

        [Description("Hậu vệ")]
        CB = 2,

        [Description("Thủ môn")]
        GK = 3,
    }
}