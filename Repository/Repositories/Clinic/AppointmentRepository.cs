using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Repository.Interfaces.Clinic;
using static Repository.DataModels.Clinic.AppointmentDTO;

namespace Repository.Repositories.Clinic
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDbConnection _dbConnection;

        public AppointmentRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public AppointmentResponse Create(CreateAppointmentRequest request)
        {
            string sql = @"
                INSERT INTO appointments (tenant_id, patient_id, practitioner_id, service_id, appointment_date, start_time, end_time, status, notes, recurrence_rule, recurrence_group_id, created_at)
                VALUES (@TenantId, @PatientId, @PractitionerId, @ServiceId, @AppointmentDate, @StartTime, @EndTime, @Status, @Notes, @RecurrenceRule, @RecurrenceGroupId, NOW())
                RETURNING id;";

            long id = _dbConnection.QuerySingle<long>(sql, new
            {
                request.TenantId,
                request.PatientId,
                request.PractitionerId,
                request.ServiceId,
                AppointmentDate = request.AppointmentDate.ToDateTime(TimeOnly.MinValue),
                StartTime = request.StartTime.ToTimeSpan(),
                EndTime = request.EndTime.ToTimeSpan(),
                request.Status,
                request.Notes,
                request.RecurrenceRule,
                request.RecurrenceGroupId
            });
            return GetById(request.TenantId, id)!;
        }

        public List<AppointmentResponse> GetByTenant(long tenantId)
        {
            string sql = BaseSelectSql() + @"
                WHERE a.tenant_id = @TenantId
                ORDER BY a.appointment_date DESC, a.start_time;";
            return _dbConnection.Query<AppointmentResponse>(sql, new { TenantId = tenantId }).ToList();
        }

        public List<AppointmentResponse> GetByDateRange(long tenantId, DateOnly from, DateOnly to)
        {
            string sql = BaseSelectSql() + @"
                WHERE a.tenant_id = @TenantId
                  AND a.appointment_date >= @From
                  AND a.appointment_date <= @To
                ORDER BY a.appointment_date, a.start_time;";
            return _dbConnection.Query<AppointmentResponse>(sql, new { TenantId = tenantId, From = from, To = to }).ToList();
        }

        public List<AppointmentResponse> GetByPractitioner(long tenantId, long practitionerId, DateOnly? date = null)
        {
            string sql = BaseSelectSql() + @"
                WHERE a.tenant_id = @TenantId
                  AND a.practitioner_id = @PractitionerId"
                + (date.HasValue ? " AND a.appointment_date = @Date" : "") + @"
                ORDER BY a.appointment_date, a.start_time;";
            return _dbConnection.Query<AppointmentResponse>(sql, new { TenantId = tenantId, PractitionerId = practitionerId, Date = date }).ToList();
        }

        public List<AppointmentResponse> GetByPatient(long tenantId, long patientId)
        {
            string sql = BaseSelectSql() + @"
                WHERE a.tenant_id = @TenantId
                  AND a.patient_id = @PatientId
                ORDER BY a.appointment_date DESC, a.start_time;";
            return _dbConnection.Query<AppointmentResponse>(sql, new { TenantId = tenantId, PatientId = patientId }).ToList();
        }

        public AppointmentResponse? GetById(long tenantId, long id)
        {
            string sql = BaseSelectSql() + @"
                WHERE a.id = @Id AND a.tenant_id = @TenantId;";
            return _dbConnection.QuerySingleOrDefault<AppointmentResponse>(sql, new { Id = id, TenantId = tenantId });
        }

        public AppointmentResponse? Update(long tenantId, long id, UpdateAppointmentRequest request)
        {
            string sql = @"
                UPDATE appointments
                SET patient_id = COALESCE(@PatientId, patient_id),
                    practitioner_id = COALESCE(@PractitionerId, practitioner_id),
                    service_id = COALESCE(@ServiceId, service_id),
                    appointment_date = COALESCE(@AppointmentDate, appointment_date),
                    start_time = COALESCE(@StartTime, start_time),
                    end_time = COALESCE(@EndTime, end_time),
                    status = COALESCE(@Status, status),
                    notes = COALESCE(@Notes, notes),
                    recurrence_rule = COALESCE(@RecurrenceRule, recurrence_rule),
                    updated_at = NOW()
                WHERE id = @Id AND tenant_id = @TenantId;";

            int affected = _dbConnection.Execute(sql, new
            {
                request.PatientId,
                request.PractitionerId,
                request.ServiceId,
                AppointmentDate = request.AppointmentDate?.ToDateTime(TimeOnly.MinValue),
                StartTime = request.StartTime?.ToTimeSpan(),
                EndTime = request.EndTime?.ToTimeSpan(),
                request.Status,
                request.Notes,
                request.RecurrenceRule,
                Id = id,
                TenantId = tenantId
            });

            return affected > 0 ? GetById(tenantId, id) : null;
        }

        public bool Delete(long tenantId, long id)
        {
            string sql = "DELETE FROM appointments WHERE id = @Id AND tenant_id = @TenantId;";
            return _dbConnection.Execute(sql, new { Id = id, TenantId = tenantId }) > 0;
        }

        private static string BaseSelectSql()
        {
            return @"
                SELECT a.id, a.tenant_id, a.patient_id,
                       CONCAT(p.first_name, ' ', p.last_name) AS patient_name,
                       a.practitioner_id,
                       CONCAT(pr.first_name, ' ', pr.last_name) AS practitioner_name,
                       a.service_id, s.name AS service_name,
                       a.appointment_date, a.start_time, a.end_time, a.status, a.notes,
                       a.recurrence_rule, a.recurrence_group_id, a.created_at, a.updated_at
                FROM appointments a
                JOIN patients p ON p.id = a.patient_id
                JOIN practitioners pr ON pr.id = a.practitioner_id
                JOIN clinic_services s ON s.id = a.service_id";
        }
    }
}
