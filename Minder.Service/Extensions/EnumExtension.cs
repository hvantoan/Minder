using System;
using System.ComponentModel;

namespace Minder.Service.Extensions {
    public static class EnumExtension {

        public static string Description(this Enum value) {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return "Chưa xác định";

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute is DescriptionAttribute ? ((DescriptionAttribute)attribute).Description : value.ToString();
        }
    }
}