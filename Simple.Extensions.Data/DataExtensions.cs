using System;
using System.Data.Common;

namespace Simple.Extensions.Data
{
    public static class DataExtensions
    {
        public static T GetValue<T>(this DbDataReader reader, string fieldName, T defaultValue)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            try
            {
                var column = reader.GetOrdinal(fieldName);
                return reader.IsDBNull(column) ? defaultValue : reader.GetFieldValue<T>(column);
            }
            catch (IndexOutOfRangeException)
            {
                throw new IndexOutOfRangeException($"Field {fieldName} does not exist.");
            }
            catch (Exception ex)
            {
                throw new Exception($"{fieldName} - {ex.Message}", ex);
            }
        }

        public static T GetValue<T>(this DbDataReader reader, string fieldName)
        {
            return reader.GetValue<T>(fieldName, default);
        }

        public static T GetValueIfColumnExists<T>(this DbDataReader reader, string fieldName)
        {
            try
            {
                return reader.GetValue<T>(fieldName, default);
            }
            catch (IndexOutOfRangeException)
            {
                return default;
            }
        }
        public static bool FieldExists(this DbDataReader reader, string fieldName)
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
