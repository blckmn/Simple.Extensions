using System;
using System.Data;

namespace Simple.Extensions.Data
{
    public static class DataExtensions
    {
        public static T GetValue<T>(this IDataReader reader, string fieldName)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            try
            {
                int index = reader.GetOrdinal(fieldName);

                var result = reader.GetValue(index);

                if (result == DBNull.Value)
                {
                    return default;
                }

                return (T)result;
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception($"Field not found: {fieldName}");
            }
        }

        public static bool FieldExists(this IDataReader reader, string fieldName)
        {
            try
            {
                for (var pos = 0; pos < reader.FieldCount; pos++)
                {
                    if (reader.GetName(pos).Equals(fieldName, StringComparison.CurrentCultureIgnoreCase))

                    {
                        return true;
                    }
                }

                return false;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }
    }
}
