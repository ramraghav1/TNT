using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.AgentDTO;

namespace Repository.Repositories.Remittance
{
    public class AgentRepository : IAgentRepository
    {
        private readonly IDbConnection _dbConnection;

        public AgentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public AgentResponse Create(CreateAgentRequest request)
        {
            string sql = @"
                INSERT INTO agents (name, country_id, agent_type, address, contact_person, contact_email, contact_phone, created_at)
                VALUES (@Name, @CountryId, @AgentType, @Address, @ContactPerson, @ContactEmail, @ContactPhone, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(id)!;
        }

        public List<AgentResponse> GetAll()
        {
            string sql = @"
                SELECT a.id, a.name, a.country_id, c.name AS country_name,
                       a.agent_type, a.address,
                       a.contact_person, a.contact_email, a.contact_phone, a.is_active, a.created_at
                FROM agents a
                JOIN countries c ON c.id = a.country_id
                ORDER BY a.name;";
            return _dbConnection.Query<AgentResponse>(sql).ToList();
        }

        public List<AgentResponse> GetByCountry(long countryId)
        {
            string sql = @"
                SELECT a.id, a.name, a.country_id, c.name AS country_name,
                       a.agent_type, a.address,
                       a.contact_person, a.contact_email, a.contact_phone, a.is_active, a.created_at
                FROM agents a
                JOIN countries c ON c.id = a.country_id
                WHERE a.country_id = @CountryId
                ORDER BY a.name;";
            return _dbConnection.Query<AgentResponse>(sql, new { CountryId = countryId }).ToList();
        }

        public AgentResponse? GetById(long id)
        {
            string sql = @"
                SELECT a.id, a.name, a.country_id, c.name AS country_name,
                       a.agent_type, a.address,
                       a.contact_person, a.contact_email, a.contact_phone, a.is_active, a.created_at
                FROM agents a
                JOIN countries c ON c.id = a.country_id
                WHERE a.id = @Id;";
            return _dbConnection.QuerySingleOrDefault<AgentResponse>(sql, new { Id = id });
        }

        public AgentResponse? Update(long id, UpdateAgentRequest request)
        {
            string sql = @"
                UPDATE agents
                SET name = COALESCE(@Name, name),
                    agent_type = COALESCE(@AgentType, agent_type),
                    address = COALESCE(@Address, address),
                    contact_person = COALESCE(@ContactPerson, contact_person),
                    contact_email = COALESCE(@ContactEmail, contact_email),
                    contact_phone = COALESCE(@ContactPhone, contact_phone),
                    is_active = COALESCE(@IsActive, is_active),
                    updated_at = NOW()
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.Name,
                request.ContactPerson,
                request.ContactEmail,
                request.ContactPhone,
                request.IsActive,
                Id = id
            });

            return affected > 0 ? GetById(id) : null;
        }

        public bool Delete(long id)
        {
            string sql = "DELETE FROM agents WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
