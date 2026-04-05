using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Repository.DataModels.Organization;
using Repository.Interfaces;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly IDbConnection _dbConnection;

    public OrganizationRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<long> InsertAsync(CreateOrganizationDTO entity)
    {
        entity.created_by = 1;
        
        string sql = @"
            INSERT INTO organization 
            (organization_name, organization_type, country_iso3, status, contact_person, contact_email, contact_phone, created_at, created_by,updated_by,updated_at)
            VALUES 
           (@organization_name, @organization_type, @country_iso3, @status, @contact_person, @contact_email, @contact_phone,now(), @created_by,@created_by,now())
            RETURNING id;
         ";

        return await _dbConnection.ExecuteScalarAsync<long>(sql, entity);
    }

    public async Task<int> UpdateAsync(CreateOrganizationDTO entity)
    {
        string sql = @"
            UPDATE organization
            SET 
                organization_name = @OrganizationName,
                organization_type = @OrganizationType,
                country_iso3 = @CountryIso3,
                status = @Status,
                contact_person = @ContactPerson,
                contact_email = @ContactEmail,
                contact_phone = @ContactPhone
            WHERE id = @OrganizationId;
        ";

        return await _dbConnection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int organizationId)
    {
        string sql = "DELETE FROM organization WHERE id = @OrganizationId;";
        return await _dbConnection.ExecuteAsync(sql, new { OrganizationId = organizationId });
    }

    public async Task<IEnumerable<OrganizationDetailDTO>> ListAsync()
    {
        string sql = "SELECT id, organization_name, organization_type, country_iso3, status, contact_person, contact_email, contact_phone FROM organization ORDER BY id;";
        return await _dbConnection.QueryAsync<OrganizationDetailDTO>(sql);
    }

    public async Task<OrganizationSetupResultDTO> SetupOrganizationWithManagerAsync(SetupOrganizationDTO request)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        using var transaction = _dbConnection.BeginTransaction();
        try
        {
            // 1. Create organization
            string orgSql = @"
                INSERT INTO organization 
                (organization_name, organization_type, country_iso3, status, contact_person, contact_email, contact_phone, created_at, created_by, updated_by, updated_at)
                VALUES 
                (@OrganizationName, @OrganizationType, @CountryIso3, @Status, @ContactPerson, @ContactEmail, @ContactPhone, now(), 1, 1, now())
                RETURNING id;";

            long orgId = await _dbConnection.ExecuteScalarAsync<long>(orgSql, new
            {
                request.OrganizationName,
                request.OrganizationType,
                request.CountryIso3,
                Status = "ACTIVE",
                request.ContactPerson,
                request.ContactEmail,
                request.ContactPhone
            }, transaction);

            // 2. Create userinformation with org_id
            string userSql = @"
                INSERT INTO userinformation 
                (userfullname, address, emailaddress, mobilenumber, org_id, createdat, updatedat)
                VALUES 
                (@ManagerFullName, @ManagerAddress, @ManagerEmail, @ManagerPhone, @OrgId, now(), now())
                RETURNING userid;";

            int userId = await _dbConnection.ExecuteScalarAsync<int>(userSql, new
            {
                request.ManagerFullName,
                ManagerAddress = request.ManagerAddress ?? "",
                request.ManagerEmail,
                request.ManagerPhone,
                OrgId = orgId
            }, transaction);

            // 3. Create logindetail with BCrypt password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.ManagerPassword);
            string loginSql = @"
                INSERT INTO logindetail 
                (userid, username, password, org_id, createdat, updatedat)
                VALUES 
                (@UserId, @Username, @PasswordHash, @OrgId, now(), now());";

            await _dbConnection.ExecuteAsync(loginSql, new
            {
                UserId = userId,
                Username = request.ManagerEmail,
                PasswordHash = passwordHash,
                OrgId = orgId
            }, transaction);

            transaction.Commit();

            return new OrganizationSetupResultDTO
            {
                OrganizationId = orgId,
                UserId = userId,
                OrganizationName = request.OrganizationName,
                ManagerEmail = request.ManagerEmail
            };
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
