using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.AdministrativeDivisionDTO;

namespace Repository.Repositories.Remittance
{
    public class AdministrativeDivisionRepository : IAdministrativeDivisionRepository
    {
        private readonly IDbConnection _dbConnection;

        public AdministrativeDivisionRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public List<AdministrativeDivisionResponse> GetByCountry(long countryId)
        {
            string sql = @"
                SELECT id, country_id, name, code, division_level, parent_id, is_active, created_at
                FROM administrative_divisions
                WHERE country_id = @CountryId AND is_active = true
                ORDER BY division_level, name;";

            return _dbConnection.Query<AdministrativeDivisionResponse>(sql, new { CountryId = countryId }).ToList();
        }

        public List<AdministrativeDivisionResponse> GetByCountryAndLevel(long countryId, int level)
        {
            string sql = @"
                SELECT id, country_id, name, code, division_level, parent_id, is_active, created_at
                FROM administrative_divisions
                WHERE country_id = @CountryId AND division_level = @Level AND is_active = true
                ORDER BY name;";

            return _dbConnection.Query<AdministrativeDivisionResponse>(sql, new { CountryId = countryId, Level = level }).ToList();
        }

        public List<AdministrativeDivisionResponse> GetChildren(long parentId)
        {
            string sql = @"
                SELECT id, country_id, name, code, division_level, parent_id, is_active, created_at
                FROM administrative_divisions
                WHERE parent_id = @ParentId AND is_active = true
                ORDER BY name;";

            return _dbConnection.Query<AdministrativeDivisionResponse>(sql, new { ParentId = parentId }).ToList();
        }
    }
}
