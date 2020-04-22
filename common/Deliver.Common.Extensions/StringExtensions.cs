using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static bool HasValue(this string str, bool countWhiteSpace = false)
            => countWhiteSpace ? !string.IsNullOrEmpty(str) : !string.IsNullOrWhiteSpace(str);

        public static bool IsEmpty(this string str, bool countWhiteSpace = false)
            => countWhiteSpace ? string.IsNullOrEmpty(str) : string.IsNullOrWhiteSpace(str);

        public static string ToTitleCase(this string str)
        {
            var s = string.Empty;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (i == 0 && !char.IsUpper(c))
                    c = char.ToUpper(c);
                else if (i > 0 && char.IsUpper(c))
                    c = char.ToLower(c);
                s += c;
            }
            return s;
        }

        public static string LimitTo(this string str, int length, string postFix = null)
            => new string(str.Take(postFix.HasValue() ? length - postFix.Length : length).ToArray()) + postFix;
    }
}
