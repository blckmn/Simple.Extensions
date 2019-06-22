using System;
using System.Linq;
using System.Collections.Generic;

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

            if (list.Contains(value, StringComparer.CurrentCultureIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static bool EqualsCi(this string value, string compare)
        {
            if (value == null || compare == null)
            {
                return false;
            }

            if (value.Equals(compare, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static string[] ComplimentarySet(this string[] value, params string[] list)
        {
            if (value == null || list == null)
            {
                return new string[0];
            }

            var result = new List<string>();
            foreach(var item in list)
            {
                if (item.IsAny(value))
                {
                    result.Add(item);
                }
            }
            return result.ToArray();
        }
    }
}
