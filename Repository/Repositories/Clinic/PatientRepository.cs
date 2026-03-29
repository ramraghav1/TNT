using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Clinic;
using static Repository.DataModels.Clinic.PatientDTO;

namespace Repository.Repositories.Clinic
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnection _dbConnection;

        public PatientRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public PatientResponse Create(CreatePatientRequest request)
        {
            string sql = @"
                INSERT INTO patients (tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address, medical_notes, allergies, emergency_contact_name, emergency_contact_phone, created_at)
                VALUES (@TenantId, @FirstName, @LastName, @Email, @Phone, @DateOfBirth, @Gender, @Address, @MedicalNotes, @Allergies, @EmergencyContactName, @EmergencyContactPhone, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, new
            {
                request.TenantId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.Phone,
                DateOfBirth = request.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                request.Gender,
                request.Address,
                request.MedicalNotes,
                request.Allergies,
                request.EmergencyContactName,
                request.EmergencyContactPhone
            });
            return GetById(request.TenantId, id)!;
        }

        public List<PatientResponse> GetByTenant(long tenantId)
        {
            string sql = @"
                SELECT id, tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address,
                       medical_notes, allergies, emergency_contact_name, emergency_contact_phone, is_active, created_at, updated_at
                FROM patients
                WHERE tenant_id = @TenantId
                ORDER BY first_name, last_name;";
            return _dbConnection.Query<PatientResponse>(sql, new { TenantId = tenantId }).ToList();
        }

        public PatientResponse? GetById(long tenantId, long id)
        {
            string sql = @"
                SELECT id, tenant_id, first_name, last_name, email, phone, date_of_birth, gender, address,
                       medical_notes, allergies, emergency_contact_name, emergency_contact_phone, is_active, created_at, updated_at
                FROM patients
                WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.QuerySingleOrDefault<PatientResponse>(sql, new { Id = id, TenantId = tenantId });
        }

        public PatientResponse? Update(long tenantId, long id, UpdatePatientRequest request)
        {
            string sql = @"
                UPDATE patients
                SET first_name = COALESCE(@FirstName, first_name),
                    last_name = COALESCE(@LastName, last_name),
                    email = COALESCE(@Email, email),
                    phone = COALESCE(@Phone, phone),
                    date_of_birth = COALESCE(@DateOfBirth, date_of_birth),
                    gender = COALESCE(@Gender, gender),
                    address = COALESCE(@Address, address),
                    medical_notes = COALESCE(@MedicalNotes, medical_notes),
                    allergies = COALESCE(@Allergies, allergies),
                    emergency_contact_name = COALESCE(@EmergencyContactName, emergency_contact_name),
                    emergency_contact_phone = COALESCE(@EmergencyContactPhone, emergency_contact_phone),
                    is_active = COALESCE(@IsActive, is_active),
                    updated_at = NOW()
                WHERE id = @Id AND tenant_id = @TenantId;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.FirstName,
                request.LastName,
                request.Email,
                request.Phone,
                DateOfBirth = request.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                request.Gender,
                request.Address,
                request.MedicalNotes,
                request.Allergies,
                request.EmergencyContactName,
                request.EmergencyContactPhone,
                request.IsActive,
                Id = id,
                TenantId = tenantId
            });

            return affected > 0 ? GetById(tenantId, id) : null;
        }

        public bool Delete(long tenantId, long id)
        {
            string sql = "DELETE FROM patients WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.Execute(sql, new { Id = id, TenantId = tenantId }) > 0;
        }
    }
}
