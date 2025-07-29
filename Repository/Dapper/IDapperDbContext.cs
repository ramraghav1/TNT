using System;
namespace Repository.Dapper
{
	public interface IDapperDbContext
	{
        // Execute (for INSERT, UPDATE, DELETE)
        int Execute(string sql, object? param = null);
        Task<int> ExecuteAsync(string sql, object? param = null);

        // Query multiple rows
        IEnumerable<T> Query<T>(string sql, object? param = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);

        // Query a single row
        T QuerySingle<T>(string sql, object? param = null);
        Task<T> QuerySingleAsync<T>(string sql, object? param = null);

        // Query a single row or default
        T? QuerySingleOrDefault<T>(string sql, object? param = null);
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null);
    }
}

