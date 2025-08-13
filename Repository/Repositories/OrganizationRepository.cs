using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Repository.DataModels.Organization;
using Repository.Interfaces;

public class OrganizationRepository: IOrganizationRepository
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
            RETURNING organization_id;
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
            WHERE organization_id = @OrganizationId;
        ";

        return await _dbConnection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int organizationId)
    {
        string sql = "DELETE FROM organization WHERE organization_id = @OrganizationId;";
        return await _dbConnection.ExecuteAsync(sql, new { OrganizationId = organizationId });
    }

    public async Task<IEnumerable<OrganizationDetailDTO>> ListAsync()
    {
        string sql = "SELECT * FROM organization ORDER BY organization_id;";
        return await _dbConnection.QueryAsync<OrganizationDetailDTO>(sql);
    }
}
