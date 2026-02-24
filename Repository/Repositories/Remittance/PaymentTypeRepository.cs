using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.PaymentTypeDTO;

namespace Repository.Repositories.Remittance
{
    public class PaymentTypeRepository : IPaymentTypeRepository
    {
        private readonly IDbConnection _dbConnection;

        public PaymentTypeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public PaymentTypeResponse Create(CreatePaymentTypeRequest request)
        {
            string sql = @"
                INSERT INTO payment_types (name, description, created_at)
                VALUES (@Name, @Description, NOW())
                RETURNING id, name, description, is_active, created_at;";

            return _dbConnection.QuerySingle<PaymentTypeResponse>(sql, request);
        }

        public List<PaymentTypeResponse> GetAll()
        {
            string sql = "SELECT id, name, description, is_active, created_at FROM payment_types ORDER BY name;";
            return _dbConnection.Query<PaymentTypeResponse>(sql).ToList();
        }

        public PaymentTypeResponse? GetById(long id)
        {
            string sql = "SELECT id, name, description, is_active, created_at FROM payment_types WHERE id = @Id;";
            return _dbConnection.QuerySingleOrDefault<PaymentTypeResponse>(sql, new { Id = id });
        }

        public PaymentTypeResponse? Update(long id, UpdatePaymentTypeRequest request)
        {
            string sql = @"
                UPDATE payment_types
                SET name = COALESCE(@Name, name),
                    description = COALESCE(@Description, description),
                    is_active = COALESCE(@IsActive, is_active)
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Name,
                request.Description,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM payment_types WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
