using System.Globalization;
using System.Linq;
using System.Text;

namespace Minder.Service.Extensions {

    public static class StringExtension {

        public static string UnsignedUnicode(this string text) {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            var chars = text.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        public static string GetLast(this string text, int length = 4) {
            if (string.IsNullOrWhiteSpace(text) || length >= text.Length)
                return text;
            return text[^length..];
        }

        public static string ReplaceSpace(this string text, bool isUnsignedUnicode = false) {
            if (text == null) return "";
            text = text.Trim();

            while (text.Contains("  ")) {
                text = text.Replace("  ", " ");
            }
            if (isUnsignedUnicode) {
                text = text.ToLower().UnsignedUnicode();
            }
            return text;
        }

        public static string GetSumary(this string text) {
            var textArr = text.ToArray();
            var res = new StringBuilder();

            var isGet = false;
            foreach (var character in textArr) {
                if (isGet) res.Append(character);

                if (character == ' ') {
                    isGet = true;
                } else isGet = false;
            }

            return res.ToString().ToLower();
        }
    }
}