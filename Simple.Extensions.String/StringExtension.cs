using System;
using System.Linq;

namespace Simple.Extensions.String
{
    public static class StringExtension
    {
        public static bool IsAny(this string value, params string[] list)
        {
            if (value == null || list == null)
            {
                return false;
            }

            return list.Contains(value, StringComparer.CurrentCultureIgnoreCase);
        }

        public static bool EqualsCi(this string value, string compare)
        {
            if (value == null || compare == null)
            {
                return false;
            }

            return value.Equals(compare, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string[] ComplimentarySet(this string[] value, params string[] list)
        {
            if (value == null || list == null)
            {
                return new string[0];
            }

            return list.Where(item => item.IsAny(value)).ToArray();
        }

        public static string Truncate(this string value, int count)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Length <= count ? value : value.Substring(0, count);
        }

        public static string NullIfEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
