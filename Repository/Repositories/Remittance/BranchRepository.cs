using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Remittance;
using static Repository.DataModels.Remittance.BranchDTO;

namespace Repository.Repositories.Remittance
{
    public class BranchRepository : IBranchRepository
    {
        private readonly IDbConnection _dbConnection;

        public BranchRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public BranchResponse Create(CreateBranchRequest request)
        {
            string sql = @"
                INSERT INTO branches (agent_id, branch_name, branch_code, address, state, district, locallevel, ward_number, zipcode, contact_person, contact_email, contact_phone, created_at)
                VALUES (@AgentId, @BranchName, @BranchCode, @Address, @State, @District, @Locallevel, @WardNumber, @Zipcode, @ContactPerson, @ContactEmail, @ContactPhone, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, request);
            return GetById(id)!;
        }

        public List<BranchResponse> GetAll()
        {
            string sql = @"
                SELECT b.id, b.agent_id, a.name AS agent_name,
                       b.branch_name, b.branch_code, b.address, b.state, b.district,
                       b.locallevel, b.ward_number, b.zipcode,
                       b.contact_person, b.contact_email, b.contact_phone,
                       b.is_active, b.created_at
                FROM branches b
                JOIN agents a ON a.id = b.agent_id
                ORDER BY b.branch_name;";
            return _dbConnection.Query<BranchResponse>(sql).ToList();
        }

        public List<BranchResponse> GetByAgent(long agentId)
        {
            string sql = @"
                SELECT b.id, b.agent_id, a.name AS agent_name,
                       b.branch_name, b.branch_code, b.address, b.state, b.district,
                       b.locallevel, b.ward_number, b.zipcode,
                       b.contact_person, b.contact_email, b.contact_phone,
                       b.is_active, b.created_at
                FROM branches b
                JOIN agents a ON a.id = b.agent_id
                WHERE b.agent_id = @AgentId
                ORDER BY b.branch_name;";
            return _dbConnection.Query<BranchResponse>(sql, new { AgentId = agentId }).ToList();
        }

        public BranchResponse? GetById(long id)
        {
            string sql = @"
                SELECT b.id, b.agent_id, a.name AS agent_name,
                       b.branch_name, b.branch_code, b.address, b.state, b.district,
                       b.locallevel, b.ward_number, b.zipcode,
                       b.contact_person, b.contact_email, b.contact_phone,
                       b.is_active, b.created_at
                FROM branches b
                JOIN agents a ON a.id = b.agent_id
                WHERE b.id = @Id;";
            return _dbConnection.QuerySingleOrDefault<BranchResponse>(sql, new { Id = id });
        }

        public BranchResponse? Update(long id, UpdateBranchRequest request)
        {
            string sql = @"
                UPDATE branches
                SET branch_name = COALESCE(@BranchName, branch_name),
                    branch_code = COALESCE(@BranchCode, branch_code),
                    address = COALESCE(@Address, address),
                    state = COALESCE(@State, state),
                    district = COALESCE(@District, district),
                    locallevel = COALESCE(@Locallevel, locallevel),
                    ward_number = COALESCE(@WardNumber, ward_number),
                    zipcode = COALESCE(@Zipcode, zipcode),
                    contact_person = COALESCE(@ContactPerson, contact_person),
                    contact_email = COALESCE(@ContactEmail, contact_email),
                    contact_phone = COALESCE(@ContactPhone, contact_phone),
                    is_active = COALESCE(@IsActive, is_active),
                    updated_at = NOW()
                WHERE id = @Id;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.BranchName,
                request.BranchCode,
                request.Address,
                request.State,
                request.District,
                request.Locallevel,
                request.WardNumber,
                request.Zipcode,
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
            string sql = "DELETE FROM branches WHERE id = @Id;";
            return _dbConnection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
