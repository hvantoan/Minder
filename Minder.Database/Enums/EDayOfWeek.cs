using System.ComponentModel;

namespace Minder.Database.Enums {

    public enum EDayOfWeek {

        [Description("Chủ nhật ")]
        Sunday = 0,

        [Description("Thứ 2")]
        Monday = 1,

        [Description("Thứ 3")]
        Tuesday = 2,

        [Description("Thứ 4")]
        Wednesday = 3,

        [Description("Thứ 5")]
        Thursday = 4,

        [Description("Thứ 6")]
        Friday = 5,

        [Description("Thứ 7")]
        Saturday = 6
    }
}