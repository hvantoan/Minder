using System.ComponentModel;

namespace Minder.Database.Enums {

    public enum ETime {
        [Description("12AM - 6AM")]
        OptionOne,

        [Description("6AM - 12PM")]
        OptionTwo,

        [Description("12PM - 6PM")]
        OptionThree,

        [Description("6PM - 12AM")]
        OptionFour,
    }
}