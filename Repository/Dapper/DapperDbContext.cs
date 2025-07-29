using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
namespace Repository.Dapper
{
    public class DapperDbContext : IDapperDbContext, IDisposable
    {
        private readonly IDbConnection _connection;

        public DapperDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public IDbConnection Connection => _connection;

        // 🧠 Execute a command (INSERT, UPDATE, DELETE)
        public int Execute(string sql, object? param = null)
        {
            return _connection.Execute(sql, param);
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            return await _connection.ExecuteAsync(sql, param);
        }

        // 🧠 Query multiple rows
        public IEnumerable<T> Query<T>(string sql, object? param = null)
        {
            return _connection.Query<T>(sql, param);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            return await _connection.QueryAsync<T>(sql, param);
        }

        // 🧠 Query a single row
        public T QuerySingle<T>(string sql, object? param = null)
        {
            return _connection.QuerySingle<T>(sql, param);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object? param = null)
        {
            return await _connection.QuerySingleAsync<T>(sql, param);
        }

        // 🧠 Query a single or default (for optional results)
        public T? QuerySingleOrDefault<T>(string sql, object? param = null)
        {
            return _connection.QuerySingleOrDefault<T>(sql, param);
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
        {
            return await _connection.QuerySingleOrDefaultAsync<T>(sql, param);
        }

        // 🧹 Cleanup
        public void Dispose()
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();

            _connection.Dispose();
        }
    }
}
