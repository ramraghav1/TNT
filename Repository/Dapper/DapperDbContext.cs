using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Repository.Dapper
{
    public class DapperDbContext : IDapperDbContext
    {
        private readonly IDbConnection _connection;

        public DapperDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _connection = new SqlConnection(connectionString);
            _connection.Open(); // Open once and reuse
        }

        public IDbConnection Connection => _connection;

        public void Dispose()
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();

            _connection.Dispose();
        }
    }
}
