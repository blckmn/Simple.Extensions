using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Simple.Extensions.Mssql
{
    public class MssqlRepository
    {
        public string ConnectionString { get; set; }

        protected SqlConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new ArgumentException(nameof(ConnectionString));
            }

            return new SqlConnection(ConnectionString);
        }

        public async Task Transaction(Func<SqlTransaction, Task> paramAction)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    await paramAction(transaction);

                    transaction.Commit();
                }
            }
        }

        public async Task<T> GetRecord<T>(string sql, Func<DbDataReader, T> resultFunc)
        {
            return await GetRecord(sql, null, resultFunc);
        }

        public async Task<T> GetRecord<T>(string sql, Action<SqlParameterCollection> paramAction, Func<DbDataReader, T> resultFunc)
        {
            if (resultFunc == null)
            {
                throw new ArgumentNullException(nameof(resultFunc));
            }

            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    paramAction?.Invoke(command.Parameters);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? resultFunc(reader) : default;
                    }
                }
            }
        }

        public async Task<T> GetRecord<T>(string sql, Action<SqlParameterCollection> paramAction, Func<DbDataReader, T> resultFunc, SqlTransaction transaction)
        {
            if (resultFunc == null)
            {
                throw new ArgumentNullException(nameof(resultFunc));
            }

            if (transaction == null)
            {
                return await GetRecord(sql, paramAction, resultFunc);
            }

            using (var command = new SqlCommand(sql, transaction.Connection, transaction))
            {
                paramAction?.Invoke(command.Parameters);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    return await reader.ReadAsync() ? resultFunc.Invoke(reader) : default;
                }
            }
        }

        public async Task<List<T>> GetRecords<T>(string sql, Func<DbDataReader, T> resultFunc)
        {
            return await GetRecords(sql, null, resultFunc);
        }

        public async Task<List<T>> GetRecords<T>(string sql, Action<SqlParameterCollection> paramAction, Func<DbDataReader, T> resultFunc)
        {
            if (resultFunc == null)
            {
                throw new ArgumentNullException(nameof(resultFunc));
            }

            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    paramAction?.Invoke(command.Parameters);
                    using (var reader = command.ExecuteReader())
                    {
                        var result = new List<T>();
                        while (await reader.ReadAsync())
                        {
                            result.Add(resultFunc.Invoke(reader));
                        }

                        return result;
                    }
                }
            }
        }

        public async Task<long> NonQuery(string sql, Action<SqlParameterCollection> paramAction)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    paramAction?.Invoke(command.Parameters);
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<long> NonQuery(string sql, Action<SqlParameterCollection> paramAction, SqlTransaction transaction)
        {
            if (transaction == null)
            {
                return await NonQuery(sql, paramAction);
            }

            using (var command = new SqlCommand(sql, transaction.Connection, transaction))
            {
                paramAction?.Invoke(command.Parameters);
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
