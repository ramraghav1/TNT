using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.ConfigurationTypeDTO;

namespace Repository.Repositories.Remittance
{
    public class ConfigurationTypeRepository : IConfigurationTypeRepository
    {
        private readonly IDbConnection _dbConnection;

        public ConfigurationTypeRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public ConfigurationTypeResponse Create(CreateConfigurationTypeRequest request)
        {
            string sql = @"
                INSERT INTO configuration_types (name, created_at)
                VALUES (@Name, NOW())
                RETURNING id, name, is_active, created_at;";

            return _dbConnection.QuerySingle<ConfigurationTypeResponse>(sql, request);
        }

        public List<ConfigurationTypeResponse> GetAll()
        {
            string sql = "SELECT id, name, is_active, created_at FROM configuration_types ORDER BY name;";
            return _dbConnection.Query<ConfigurationTypeResponse>(sql).ToList();
        }

        public ConfigurationTypeResponse? GetById(long id)
        {
            string sql = "SELECT id, name, is_active, created_at FROM configuration_types WHERE id = @Id;";
            return _dbConnection.QuerySingleOrDefault<ConfigurationTypeResponse>(sql, new { Id = id });
        }

        public ConfigurationTypeResponse? Update(long id, UpdateConfigurationTypeRequest request)
        {
            string sql = @"
                UPDATE configuration_types
                SET name      = COALESCE(@Name, name),
                    is_active = COALESCE(@IsActive, is_active)
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Name,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM configuration_types WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
