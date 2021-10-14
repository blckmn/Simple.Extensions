using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Simple.Extensions.DynamoDb
{
    public static class DynamoDbExtension
    {
        public static AttributeValue Get(this Dictionary<string, AttributeValue> item, string key)
        {
            return item.ContainsKey(key) ? item[key] : null;
        }

        public static bool AddIfNotNull(this Dictionary<string, AttributeValue> a, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            a.Add(key, new AttributeValue { S = value });
            return true;
        }

        public static string AsString(this AttributeValue value)
        {
            return value?.S;
        }

        public static Guid AsGuid(this AttributeValue value)
        {
            return value == null ? Guid.Empty : Guid.Parse(value.S);
        }

        public static bool? AsBoolean(this AttributeValue value)
        {
            return value?.BOOL;
        }

        public static int AsInteger(this AttributeValue value)
        {
            return value == null ? 0 : Convert.ToInt32(value.N);
        }

        public static DateTime? AsUtc(this AttributeValue value)
        {
            return value == null ? (DateTime?)null : DateTime.Parse(value.S).ToUniversalTime();
        }

        public static DateTime AsUtc(this AttributeValue value, DateTime defaultValue)
        {
            return value == null ? defaultValue : DateTime.Parse(value.S).ToUniversalTime();
        }

        public static DateTime AsUtcFromEpoch(this AttributeValue value, DateTime defaultValue)
        {
            return value == null ?
                defaultValue :
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Convert.ToInt32(value.N));
        }

        public static long ToEpoch(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;

        public static AttributeValue ToAttributeValue(this string value) => new AttributeValue { S = value };
        public static AttributeValue ToAttributeValue(this long value) => new AttributeValue { N = value.ToString() };
        public static AttributeValue ToAttributeValue(this int value) => new AttributeValue { N = value.ToString() };
        public static AttributeValue ToAttributeValue(this Guid value) => new AttributeValue { S = value.ToString("D") };
        public static AttributeValue ToAttributeValue(this DateTime value) => new AttributeValue { S = value.ToUniversalTime().ToString("O") };

        public static async Task<List<T>> QueryAllAsync<T>(this AmazonDynamoDBClient client, QueryRequest request, Func<Dictionary<string, AttributeValue>, T> func)
        {
            var result = new List<T>();
            while (true)
            {
                var response = await client.QueryAsync(request);
                request.ExclusiveStartKey = response.LastEvaluatedKey;
                if (response.Count > 0)
                {
                    result.AddRange(response.Items.Select(func));
                    if (response.LastEvaluatedKey != null && response.LastEvaluatedKey.Count > 0)
                    {
                        continue;
                    }
                }

                break;
            }
            return result;
        }
    }
}
