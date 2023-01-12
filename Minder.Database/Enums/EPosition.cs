using System.ComponentModel;

namespace Minder.Database.Enums {

    public enum EPosition {
        [Description("Thủ môn")]
        GK = 0,
        [Description("Tiền đạo cánh trái")]
        LW = 1,
        [Description("Tiền đạo cánh phải")]
        RW = 2,
        [Description("Tiền vệ tấn công")]
        CAM = 3,
        [Description("Tiền vệ")]
        CM = 4,
        [Description("Hậu vệ phải")]
        RB = 5,
        [Description("Hậu vệ trái")]
        LB = 6,
        [Description("Hậu vệ")]
        CB = 7,
        [Description("Tiền đạo cắm")]
        ST = 8,
    }
}