using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.ConfigurationDTO;

namespace Repository.Repositories.Remittance
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IDbConnection _dbConnection;

        public ConfigurationRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public ConfigurationResponse Create(CreateConfigurationRequest request)
        {
            string sql = @"
                INSERT INTO configurations (configuration_type_id, code, display_name, created_at)
                VALUES (@ConfigurationTypeId, @Code, @DisplayName, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(id)!;
        }

        public List<ConfigurationResponse> GetAll()
        {
            string sql = @"
                SELECT c.id, c.configuration_type_id, ct.name AS configuration_type_name,
                       c.code, c.display_name, c.is_active, c.created_at
                FROM configurations c
                JOIN configuration_types ct ON ct.id = c.configuration_type_id
                ORDER BY ct.name, c.display_name;";

            return _dbConnection.Query<ConfigurationResponse>(sql).ToList();
        }

        public List<ConfigurationResponse> GetByTypeId(long configurationTypeId)
        {
            string sql = @"
                SELECT c.id, c.configuration_type_id, ct.name AS configuration_type_name,
                       c.code, c.display_name, c.is_active, c.created_at
                FROM configurations c
                JOIN configuration_types ct ON ct.id = c.configuration_type_id
                WHERE c.configuration_type_id = @ConfigurationTypeId
                ORDER BY c.display_name;";

            return _dbConnection.Query<ConfigurationResponse>(sql, new { ConfigurationTypeId = configurationTypeId }).ToList();
        }

        public ConfigurationResponse? GetById(long id)
        {
            string sql = @"
                SELECT c.id, c.configuration_type_id, ct.name AS configuration_type_name,
                       c.code, c.display_name, c.is_active, c.created_at
                FROM configurations c
                JOIN configuration_types ct ON ct.id = c.configuration_type_id
                WHERE c.id = @Id;";

            return _dbConnection.QuerySingleOrDefault<ConfigurationResponse>(sql, new { Id = id });
        }

        public ConfigurationResponse? Update(long id, UpdateConfigurationRequest request)
        {
            string sql = @"
                UPDATE configurations
                SET code         = COALESCE(@Code, code),
                    display_name = COALESCE(@DisplayName, display_name),
                    is_active    = COALESCE(@IsActive, is_active)
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Code,
                request.DisplayName,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM configurations WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
