using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.CountryDTO;

namespace Repository.Repositories.Remittance
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IDbConnection _dbConnection;

        public CountryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public CountryResponse Create(CreateCountryRequest request)
        {
            string sql = @"
                INSERT INTO countries (name, iso2_code, iso3_code, phone_code, currency_code, currency_name, created_at)
                VALUES (@Name, @Iso2Code, @Iso3Code, @PhoneCode, @CurrencyCode, @CurrencyName, NOW())
                RETURNING id, name, iso2_code, iso3_code, phone_code, currency_code, currency_name, is_active, created_at;";

            return _dbConnection.QuerySingle<CountryResponse>(sql, request);
        }

        public List<CountryResponse> GetAll()
        {
            string sql = "SELECT id, name, iso2_code, iso3_code, phone_code, currency_code, currency_name, is_active, created_at FROM countries ORDER BY name;";
            return _dbConnection.Query<CountryResponse>(sql).ToList();
        }

        public CountryResponse? GetById(long id)
        {
            string sql = "SELECT id, name, iso2_code, iso3_code, phone_code, currency_code, currency_name, is_active, created_at FROM countries WHERE id = @Id;";
            return _dbConnection.QuerySingleOrDefault<CountryResponse>(sql, new { Id = id });
        }

        public CountryResponse? Update(long id, UpdateCountryRequest request)
        {
            string sql = @"
                UPDATE countries
                SET name = COALESCE(@Name, name),
                    phone_code = COALESCE(@PhoneCode, phone_code),
                    currency_code = COALESCE(@CurrencyCode, currency_code),
                    currency_name = COALESCE(@CurrencyName, currency_name),
                    is_active = COALESCE(@IsActive, is_active)
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Name,
                request.PhoneCode,
                request.CurrencyCode,
                request.CurrencyName,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM countries WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
