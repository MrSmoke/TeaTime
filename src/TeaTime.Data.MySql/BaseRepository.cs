namespace TeaTime.Data.MySql
{
    using System;
    using System.Threading.Tasks;
    using Dapper;
    using global::MySql.Data.MySqlClient;

    public abstract class BaseRepository
    {
        private readonly ConnectionFactory _factory;

        protected BaseRepository(ConnectionFactory factory)
        {
            _factory = factory;
        }

        protected Task<int> Insert(string sql, object obj)
        {
            return GetConnection(conn => conn.ExecuteAsync(sql, obj));
        }

        protected Task<T> SingleOrDefault<T>(string sql, object obj = null)
        {
            return GetConnection(conn => conn.QuerySingleOrDefaultAsync<T>(sql, obj));
        }

        protected async Task<T> GetConnection<T>(Func<MySqlConnection, Task<T>> func)
        {
            using (var conn = _factory.GetConnection())
            {
                return await func(conn).ConfigureAwait(false);
            }
        }
    }
}
