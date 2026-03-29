using System.Data;
using Dapper;
using Repository.DataModels;
using Repository.Interfaces;

namespace Repository.Repositories
{
    public class DemoRequestRepository : IDemoRequestRepository
    {
        private readonly IDbConnection _dbConnection;

        public DemoRequestRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<long> InsertAsync(DemoRequestDTO entity)
        {
            string sql = @"INSERT INTO demo_request (full_name, email, phone, company_name, product_interest, message)
                           VALUES (@FullName, @Email, @Phone, @CompanyName, @ProductInterest, @Message)
                           RETURNING id;";
            return await _dbConnection.ExecuteScalarAsync<long>(sql, entity);
        }

        public async Task<IEnumerable<DemoRequestDTO>> ListAsync()
        {
            string sql = @"SELECT id, full_name AS FullName, email, phone, company_name AS CompanyName,
                           product_interest AS ProductInterest, message, status, created_at AS CreatedAt
                           FROM demo_request ORDER BY created_at DESC;";
            return await _dbConnection.QueryAsync<DemoRequestDTO>(sql);
        }
    }
}
