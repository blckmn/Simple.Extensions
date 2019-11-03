using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Npgsql;

namespace Simple.Extensions.Npgsql
{
    public class NpgsqlRepository
    {
        public string ConnectionString { get; set; }

        protected NpgsqlConnection CreateConnection()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new ArgumentException(nameof(ConnectionString));
            }

            return new NpgsqlConnection(ConnectionString);
        }

        public async Task Transaction(Func<NpgsqlTransaction, Task> paramAction)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    await paramAction(transaction);

                    await transaction.CommitAsync();
                }
            }
        }

        public async Task<T> GetRecord<T>(string sql, Func<DbDataReader, T> resultFunc)
        {
            return await GetRecord(sql, null, resultFunc);
        }

        public async Task<T> GetRecord<T>(string sql, Action<NpgsqlParameterCollection> paramAction, Func<DbDataReader, T> resultFunc)
        {
            if (resultFunc == null)
            {
                throw new ArgumentNullException(nameof(resultFunc));
            }

            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    paramAction?.Invoke(command.Parameters);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return await reader.ReadAsync() ? resultFunc(reader) : default;
                    }
                }
            }
        }

        public async Task<T> GetRecord<T>(string sql, Action<NpgsqlParameterCollection> paramAction, Func<DbDataReader, T> resultFunc, NpgsqlTransaction transaction)
        {
            if (resultFunc == null)
            {
                throw new ArgumentNullException(nameof(resultFunc));
            }

            if (transaction == null)
            {
                return await GetRecord(sql, paramAction, resultFunc);
            }

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
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

        public async Task<List<T>> GetRecords<T>(string sql, Action<NpgsqlParameterCollection> paramAction, Func<DbDataReader, T> resultFunc)
        {
            if (resultFunc == null)
            {
                throw new ArgumentNullException(nameof(resultFunc));
            }

            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
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

        public async Task<long> NonQuery(string sql, Action<NpgsqlParameterCollection> paramAction)
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    paramAction?.Invoke(command.Parameters);
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<long> NonQuery(string sql, Action<NpgsqlParameterCollection> paramAction, NpgsqlTransaction transaction)
        {
            if (transaction == null)
            {
                return await NonQuery(sql, paramAction);
            }

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                paramAction?.Invoke(command.Parameters);
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
